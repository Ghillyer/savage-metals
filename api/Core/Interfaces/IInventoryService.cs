using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.Interfaces;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(InventoryStatus? status, CancellationToken cancellationToken = default);
}