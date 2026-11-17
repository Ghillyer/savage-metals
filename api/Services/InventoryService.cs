using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class InventoryService(IInventoryRepository inventoryRepository) : IInventoryService
{
    public async Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(InventoryStatus? status, CancellationToken cancellationToken)
    {
        return await inventoryRepository.GetAllAsync(status, cancellationToken);
    }
}