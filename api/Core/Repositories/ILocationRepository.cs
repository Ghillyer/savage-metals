using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface ILocationRepository
{
    Task<IReadOnlyList<Location>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
}