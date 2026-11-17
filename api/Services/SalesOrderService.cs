using SavageMetals.Api.Core.DTOs.SalesOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class SalesOrderService(
    ISalesOrderRepository salesOrderRepository,
    IInventoryRepository inventoryRepository,
    IVendorRepository vendorRepository,
    ICallerContext callerContext) : ISalesOrderService
{
    private const int MaxLength = 200;
    private const decimal MaxUnitPrice = 99999999999999.9999m;

    public async Task<IReadOnlyList<SalesOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await salesOrderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<SalesOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await salesOrderRepository.GetDetailAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Sales order {id} was not found.");
    }

    public async Task<SalesOrderResponse> CreateAsync(SalesOrderRequest request, CancellationToken cancellationToken)
    {
        var soNumber = NormalizeRequired(request.SoNumber, "SO number");
        var containerNumber = NormalizeRequired(request.ContainerNumber, "Container number");
        await EnsureCustomerAsync(request.CustomerId, cancellationToken);
        var lines = await BuildLinesAsync(request.Lines, cancellationToken);
        await EnsureSoNumberIsUniqueAsync(soNumber, cancellationToken);

        var salesOrder = new SalesOrder
        {
            SoNumber = soNumber,
            CustomerId = request.CustomerId,
            ContainerNumber = containerNumber,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedByUserId = callerContext.UserId,
        };

        var id = await salesOrderRepository.CreateAsync(salesOrder, lines, cancellationToken)
            ?? throw new ConflictException("One or more items were sold or processed by someone else. Reload and try again.");

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task EnsureCustomerAsync(int customerId, CancellationToken cancellationToken)
    {
        var customer = await vendorRepository.GetByIdAsync(customerId, cancellationToken);

        if (customer is null || !customer.IsActive)
        {
            throw new InvalidRequestException("Choose an active customer.");
        }

        if (customer.Type == VendorType.Supplier)
        {
            throw new InvalidRequestException($"{customer.Name} is a supplier, not a customer.");
        }
    }

    private async Task<List<SalesOrderLine>> BuildLinesAsync(IReadOnlyList<SalesOrderLineRequest> requestLines, CancellationToken cancellationToken)
    {
        if (requestLines.Count == 0)
        {
            throw new InvalidRequestException("A sales order needs at least one item.");
        }

        var ids = requestLines.Select(line => line.InventoryItemId).ToList();

        if (ids.Distinct().Count() != ids.Count)
        {
            throw new InvalidRequestException("Each item can only be added once.");
        }

        var itemsById = (await inventoryRepository.GetByIdsAsync(ids, cancellationToken)).ToDictionary(item => item.Id);
        var lines = new List<SalesOrderLine>();

        foreach (var requestLine in requestLines)
        {
            if (!itemsById.TryGetValue(requestLine.InventoryItemId, out var item))
            {
                throw new InvalidRequestException("Choose an item for each line.");
            }

            if (item.Status != InventoryStatus.Available)
            {
                throw new ConflictException($"Tag {item.TagNumber} is not available.");
            }

            if (requestLine.UnitPrice <= 0 || requestLine.UnitPrice > MaxUnitPrice || decimal.Round(requestLine.UnitPrice, 4) != requestLine.UnitPrice)
            {
                throw new InvalidRequestException("Price per lb must be positive and have no more than 4 decimal places.");
            }

            lines.Add(new SalesOrderLine
            {
                InventoryItemId = item.Id,
                UnitPrice = requestLine.UnitPrice,
            });
        }

        return lines;
    }

    private async Task EnsureSoNumberIsUniqueAsync(string soNumber, CancellationToken cancellationToken)
    {
        if (await salesOrderRepository.SoNumberExistsAsync(soNumber, cancellationToken))
        {
            throw new ConflictException($"SO number {soNumber} already exists.");
        }
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