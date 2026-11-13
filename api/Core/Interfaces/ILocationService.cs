using SavageMetals.Api.Core.DTOs.Locations;

namespace SavageMetals.Api.Core.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<LocationResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<LocationResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LocationResponse> CreateAsync(LocationRequest request, CancellationToken cancellationToken = default);
    Task<LocationResponse> UpdateAsync(int id, LocationRequest request, CancellationToken cancellationToken = default);
}