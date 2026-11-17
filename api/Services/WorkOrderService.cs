using SavageMetals.Api.Core.DTOs.WorkOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class WorkOrderService(
    IWorkOrderRepository workOrderRepository,
    IInventoryRepository inventoryRepository,
    IProductRepository productRepository,
    ILocationRepository locationRepository,
    ICallerContext callerContext) : IWorkOrderService
{
    private const int MaxLength = 200;
    private const decimal MaxWeight = 99999999999999.9999m;

    public async Task<IReadOnlyList<WorkOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await workOrderRepository.GetAllAsync(cancellationToken);
    }

    public async Task<WorkOrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await workOrderRepository.GetDetailAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Work order {id} was not found.");
    }

    public async Task<WorkOrderResponse> CreateAsync(WorkOrderRequest request, CancellationToken cancellationToken)
    {
        var createdAtUtc = DateTime.UtcNow;
        var input = await GetAvailableInputAsync(request.InputItemId, cancellationToken);
        var outputs = await BuildOutputsAsync(request.Outputs, createdAtUtc, cancellationToken);

        EnsureWeightsBalance(input, outputs);
        AllocateCost(input.TotalCost, outputs);

        var items = outputs.Select(output => output.Item).ToList();
        EnsureTagsAreUnique(items);
        await EnsureTagsAreAvailableAsync(items, cancellationToken);

        var workOrder = new WorkOrder
        {
            InputItemId = input.Id,
            CreatedAtUtc = createdAtUtc,
            CreatedByUserId = callerContext.UserId,
        };

        var id = await workOrderRepository.CreateAsync(workOrder, items, cancellationToken)
            ?? throw new ConflictException($"Tag {input.TagNumber} has changed or was already processed. Reload and try again.");

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task<InventoryItem> GetAvailableInputAsync(int inputItemId, CancellationToken cancellationToken)
    {
        var input = await inventoryRepository.GetByIdAsync(inputItemId, cancellationToken)
            ?? throw new InvalidRequestException("Choose an input item.");

        if (input.Status != InventoryStatus.Available)
        {
            throw new ConflictException($"Tag {input.TagNumber} is not available.");
        }

        var product = await productRepository.GetByIdAsync(input.ProductId, cancellationToken);

        if (product is { IsScrap: true })
        {
            throw new InvalidRequestException($"Tag {input.TagNumber} is scrap and can't be processed.");
        }

        return input;
    }

    private async Task<List<PlannedOutput>> BuildOutputsAsync(IReadOnlyList<WorkOrderOutputRequest> requestOutputs, DateTime createdAtUtc, CancellationToken cancellationToken)
    {
        if (requestOutputs.Count == 0)
        {
            throw new InvalidRequestException("A work order needs at least one output.");
        }

        var outputs = new List<PlannedOutput>();

        foreach (var requestOutput in requestOutputs)
        {
            var product = await productRepository.GetByIdAsync(requestOutput.ProductId, cancellationToken);

            if (product is null || !product.IsActive)
            {
                throw new InvalidRequestException("Each output needs an active product.");
            }

            await EnsureLocationIsActiveAsync(requestOutput.LocationId, cancellationToken);

            if (requestOutput.Weight <= 0 || requestOutput.Weight > MaxWeight || decimal.Round(requestOutput.Weight, 4) != requestOutput.Weight)
            {
                throw new InvalidRequestException("Output weight must be positive and have no more than 4 decimal places.");
            }

            var item = new InventoryItem
            {
                TagNumber = NormalizeRequired(requestOutput.TagNumber, "Tag number"),
                ProductId = requestOutput.ProductId,
                LocationId = requestOutput.LocationId,
                Weight = requestOutput.Weight,
                Status = InventoryStatus.Available,
                CreatedAtUtc = createdAtUtc,
            };

            outputs.Add(new PlannedOutput(item, product.IsScrap));
        }

        return outputs;
    }

    private async Task EnsureLocationIsActiveAsync(int locationId, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetByIdAsync(locationId, cancellationToken);

        if (location is null || !location.IsActive)
        {
            throw new InvalidRequestException("Each output needs an active location.");
        }
    }

    private static void EnsureWeightsBalance(InventoryItem input, List<PlannedOutput> outputs)
    {
        var outputWeight = outputs.Sum(output => output.Item.Weight);

        if (outputWeight != input.Weight)
        {
            throw new InvalidRequestException($"Output weights total {outputWeight:#,0.####} lbs but the input weighs {input.Weight:#,0.####} lbs.");
        }
    }

    private static void AllocateCost(decimal inputCost, List<PlannedOutput> outputs)
    {
        var costBearing = outputs.Where(output => !output.IsScrap).Select(output => output.Item).ToList();

        if (costBearing.Count == 0)
        {
            throw new InvalidRequestException("At least one output must be a non-scrap product to carry the cost.");
        }

        var costBearingWeight = costBearing.Sum(item => item.Weight);
        var allocated = 0m;

        for (var i = 0; i < costBearing.Count; i++)
        {
            var item = costBearing[i];
            var isLast = i == costBearing.Count - 1;

            item.TotalCost = isLast
                ? inputCost - allocated
                : Math.Round(inputCost * item.Weight / costBearingWeight, 2, MidpointRounding.AwayFromZero);

            allocated += item.TotalCost;
        }
    }

    private static void EnsureTagsAreUnique(List<InventoryItem> items)
    {
        var hasDuplicates = items
            .GroupBy(item => item.TagNumber, StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1);

        if (hasDuplicates)
        {
            throw new InvalidRequestException("Each output needs a different tag number.");
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

    private sealed record PlannedOutput(InventoryItem Item, bool IsScrap);
}