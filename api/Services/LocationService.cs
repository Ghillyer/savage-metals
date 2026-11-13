using SavageMetals.Api.Core.DTOs.Locations;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class LocationService(ILocationRepository locationRepository) : ILocationService
{
    private const int MaxNameLength = 200;

    public async Task<IReadOnlyList<LocationResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var locations = await locationRepository.GetAllAsync(includeInactive, cancellationToken);
        return locations.Select(ToResponse).ToList();
    }

    public async Task<LocationResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Location {id} was not found.");

        return ToResponse(location);
    }

    public async Task<LocationResponse> CreateAsync(LocationRequest request, CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(request.Name);
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);

        var location = new Location
        {
            Name = name,
            IsActive = true
        };

        location.Id = await locationRepository.CreateAsync(location, cancellationToken);
        return ToResponse(location);
    }

    public async Task<LocationResponse> UpdateAsync(int id, LocationRequest request, CancellationToken cancellationToken = default)
    {
        var location = await locationRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Location {id} was not found.");

        var name = NormalizeName(request.Name);
        await EnsureNameIsUniqueAsync(name, id, cancellationToken);

        location.Name = name;
        location.IsActive = request.IsActive;

        await locationRepository.UpdateAsync(location, cancellationToken);
        return ToResponse(location);
    }

    private static string NormalizeName(string? name)
    {
        var trimmed = name?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            throw new InvalidRequestException("Location name is required.");
        }

        if (trimmed.Length > MaxNameLength)
        {
            throw new InvalidRequestException($"Location name cannot exceed {MaxNameLength} characters.");
        }

        return trimmed;
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        if (await locationRepository.NameExistsAsync(name, excludeId, cancellationToken))
        {
            throw new ConflictException($"A location named '{name}' already exists.");
        }
    }

    private static LocationResponse ToResponse(Location location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        IsActive = location.IsActive
    };
}