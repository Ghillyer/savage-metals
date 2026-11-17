using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(InventoryStatus? status, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<string?> FindExistingTagNumberAsync(IReadOnlyList<string> tagNumbers, CancellationToken cancellationToken = default);
}