using SavageMetals.Api.Core.DTOs.PurchaseOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class PurchaseOrderService(
    IPurchaseOrderRepository purchaseOrderRepository,
    IVendorRepository vendorRepository,
    IProductRepository productRepository,
    ILocationRepository locationRepository,
    IInventoryRepository inventoryRepository,
    ICallerContext callerContext) : IPurchaseOrderService
{
    private const int MaxLength = 200;

    public async Task<IReadOnlyList<PurchaseOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await purchaseOrderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<PurchaseOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await purchaseOrderRepository.GetDetailAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Purchase order {id} was not found.");
    }

    public async Task<PurchaseOrderResponse> CreateAsync(PurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var poNumber = NormalizeRequired(request.PoNumber, "PO number");
        await EnsureVendorIsSupplierAsync(request.VendorId, cancellationToken);
        var lines = await BuildLinesAsync(request.Lines, cancellationToken);
        await EnsurePoNumberIsUniqueAsync(poNumber, null, cancellationToken);

        var purchaseOrder = new PurchaseOrder
        {
            PoNumber = poNumber,
            VendorId = request.VendorId,
            Status = PurchaseOrderStatus.Open,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = callerContext.UserId,
        };

        var id = await purchaseOrderRepository.CreateAsync(purchaseOrder, lines, cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<PurchaseOrderResponse> UpdateAsync(int id, PurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await GetOpenPurchaseOrderAsync(id, cancellationToken);
        var poNumber = NormalizeRequired(request.PoNumber, "PO number");
        await EnsureVendorIsSupplierAsync(request.VendorId, cancellationToken);
        var lines = await BuildLinesAsync(request.Lines, cancellationToken);
        await EnsurePoNumberIsUniqueAsync(poNumber, id, cancellationToken);

        purchaseOrder.PoNumber = poNumber;
        purchaseOrder.VendorId = request.VendorId;

        var updated = await purchaseOrderRepository.UpdateAsync(purchaseOrder, lines, cancellationToken);

        if (!updated)
        {
            throw new ConflictException($"Purchase order {poNumber} has already been received.");
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<PurchaseOrderResponse> ReceiveAsync(int id, ReceivePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await GetOpenPurchaseOrderAsync(id, cancellationToken);
        var lines = await purchaseOrderRepository.GetLinesAsync(id, cancellationToken);
        var linesById = lines.ToDictionary(line => line.Id);

        EnsureEveryLineIsReceivedOnce(linesById, request.Lines);

        var receivedAtUtc = DateTime.UtcNow;
        var items = new List<InventoryItem>();

        foreach (var receiveLine in request.Lines)
        {
            var line = linesById[receiveLine.PurchaseOrderLineId];
            await EnsureLocationIsActiveAsync(receiveLine.LocationId, cancellationToken);

            items.Add(new InventoryItem
            {
                TagNumber = NormalizeRequired(receiveLine.TagNumber, "Tag number"),
                ProductId = line.ProductId,
                LocationId = receiveLine.LocationId,
                Weight = line.Weight,
                TotalCost = CalculateLineTotal(line.Weight, line.UnitPrice),
                Status = InventoryStatus.Available,
                CreatedAtUtc = receivedAtUtc,
                PurchaseOrderLineId = line.Id,
            });
        }

        EnsureTagsAreUnique(items);
        await EnsureTagsAreAvailableAsync(items, cancellationToken);

        var received = await purchaseOrderRepository.ReceiveAsync(id, receivedAtUtc, items, cancellationToken);

        if (!received)
        {
            throw new ConflictException($"Purchase order {purchaseOrder.PoNumber} has changed or was already received. Reload and try again.");
        }

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task<PurchaseOrder> GetOpenPurchaseOrderAsync(int id, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Purchase order {id} was not found.");

        if (purchaseOrder.Status != PurchaseOrderStatus.Open)
        {
            throw new ConflictException($"Purchase order {purchaseOrder.PoNumber} has already been received.");
        }

        return purchaseOrder;
    }

    private async Task EnsureVendorIsSupplierAsync(int vendorId, CancellationToken cancellationToken)
    {
        var vendor = await vendorRepository.GetByIdAsync(vendorId, cancellationToken);

        if (vendor is null || !vendor.IsActive)
        {
            throw new InvalidRequestException("Choose an active vendor.");
        }

        if (vendor.Type == VendorType.Customer)
        {
            throw new InvalidRequestException($"{vendor.Name} is a customer, not a supplier.");
        }
    }

    private async Task<List<PurchaseOrderLine>> BuildLinesAsync(IReadOnlyList<PurchaseOrderLineRequest> requestLines, CancellationToken cancellationToken)
    {
        if (requestLines.Count == 0)
        {
            throw new InvalidRequestException("A purchase order needs at least one line.");
        }

        const decimal maxValue = 99999999999999.9999m;
        const decimal maxOrderTotal = 9999999999999999.99m;

        var lines = new List<PurchaseOrderLine>();
        decimal orderTotal = 0;

        foreach (var requestLine in requestLines)
        {
            await EnsureProductIsActiveAsync(requestLine.ProductId, cancellationToken);

            if (requestLine.Grade is not { } grade || !Enum.IsDefined(grade))
            {
                throw new InvalidRequestException("Each line needs a valid grade.");
            }

            if (requestLine.Weight <= 0 || requestLine.Weight > maxValue || decimal.Round(requestLine.Weight, 4) != requestLine.Weight)
            {
                throw new InvalidRequestException("Weight must be positive and have no more than 4 decimal places.");
            }

            if (requestLine.UnitPrice <= 0 || requestLine.UnitPrice > maxValue || decimal.Round(requestLine.UnitPrice, 4) != requestLine.UnitPrice)
            {
                throw new InvalidRequestException("Unit price must be positive and have no more than 4 decimal places.");
            }

            var lineTotal = CalculateLineTotal(requestLine.Weight, requestLine.UnitPrice);

            if (lineTotal > maxValue)
            {
                throw new InvalidRequestException("Line total exceeds the supported amount.");
            }

            if (lineTotal > maxOrderTotal - orderTotal)
            {
                throw new InvalidRequestException("Purchase order total exceeds the supported amount.");
            }

            orderTotal += lineTotal;

            lines.Add(new PurchaseOrderLine
            {
                ProductId = requestLine.ProductId,
                Grade = grade,
                Weight = requestLine.Weight,
                UnitPrice = requestLine.UnitPrice,
            });
        }

        return lines;
    }

    private async Task EnsureProductIsActiveAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);

        if (product is null || !product.IsActive)
        {
            throw new InvalidRequestException("Each line needs an active product.");
        }
    }

    private async Task EnsureLocationIsActiveAsync(int locationId, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetByIdAsync(locationId, cancellationToken);

        if (location is null || !location.IsActive)
        {
            throw new InvalidRequestException("Each item needs an active location.");
        }
    }

    private async Task EnsurePoNumberIsUniqueAsync(string poNumber, int? excludeId, CancellationToken cancellationToken)
    {
        if (await purchaseOrderRepository.PoNumberExistsAsync(poNumber, excludeId, cancellationToken))
        {
            throw new ConflictException($"PO number {poNumber} already exists.");
        }
    }

    private static void EnsureEveryLineIsReceivedOnce(Dictionary<int, PurchaseOrderLine> linesById, IReadOnlyList<ReceiveLineRequest> receiveLines)
    {
        var requestedIds = receiveLines.Select(line => line.PurchaseOrderLineId).ToList();

        var isExactMatch = requestedIds.Count == linesById.Count
            && requestedIds.Distinct().Count() == linesById.Count
            && requestedIds.All(linesById.ContainsKey);

        if (!isExactMatch)
        {
            throw new InvalidRequestException("Every line on the purchase order must be received exactly once.");
        }
    }

    private static void EnsureTagsAreUnique(List<InventoryItem> items)
    {
        var hasDuplicates = items
            .GroupBy(item => item.TagNumber, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1);

        if (hasDuplicates)
        {
            throw new InvalidRequestException("Each item needs a different tag number.");
        }
    }

    private async Task EnsureTagsAreAvailableAsync(List<InventoryItem> items, CancellationToken cancellationToken)
    {
        var tagNumbers = items.Select(item => item.TagNumber).ToList();
        var existingTagNumber = await inventoryRepository.FindExistingTagNumberAsync(tagNumbers, cancellationToken);

        if (existingTagNumber is not null)
        {
            throw new ConflictException($"Tag {existingTagNumber} is already in use.");
        }
    }

    private static decimal CalculateLineTotal(decimal weight, decimal unitPrice)
    {
        return Math.Round(weight * unitPrice, 2, MidpointRounding.AwayFromZero);
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            throw new InvalidRequestException($"{fieldName} is required.");
        }

        if (trimmed.Length > MaxLength)
        {
            throw new InvalidRequestException($"{fieldName} must be {MaxLength} characters or fewer.");
        }

        return trimmed;
    }
}