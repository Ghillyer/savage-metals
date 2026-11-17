using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.Repositories;

public interface IInventoryRepository
{
    Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(InventoryStatus? status, CancellationToken cancellationToken = default);
    Task<string?> FindExistingTagNumberAsync(IReadOnlyList<string> tagNumbers, CancellationToken cancellationToken = default);
}