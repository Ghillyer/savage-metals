using SavageMetals.Api.Core.DTOs.Vendors;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class VendorService(IVendorRepository vendorRepository) : IVendorService
{
    private const int MaxNameLength = 200;

    public async Task<IReadOnlyList<VendorResponse>> GetAllAsync(bool includeInactive, VendorType? type, CancellationToken cancellationToken = default)
    {
        if (type.HasValue)
        {
            ValidateType(type.Value);
        }

        var vendors = await vendorRepository.GetAllAsync(includeInactive, type, cancellationToken);
        return vendors.Select(ToResponse).ToList();
    }

    public async Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vendor {id} was not found.");

        return ToResponse(vendor);
    }

    public async Task<VendorResponse> CreateAsync(VendorRequest request, CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(request.Name);
        var type = RequireType(request.Type);
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);

        var vendor = new Vendor
        {
            Name = name,
            Type = type,
            IsActive = true
        };

        vendor.Id = await vendorRepository.CreateAsync(vendor, cancellationToken);
        return ToResponse(vendor);
    }

    public async Task<VendorResponse> UpdateAsync(int id, VendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Vendor {id} was not found.");

        var name = NormalizeName(request.Name);
        var type = RequireType(request.Type);
        await EnsureNameIsUniqueAsync(name, id, cancellationToken);

        vendor.Name = name;
        vendor.Type = type;
        vendor.IsActive = request.IsActive;

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
        return ToResponse(vendor);
    }

    private static string NormalizeName(string? name)
    {
        var trimmed = name?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            throw new InvalidRequestException("Vendor name is required.");
        }

        if (trimmed.Length > MaxNameLength)
        {
            throw new InvalidRequestException($"Vendor name cannot exceed {MaxNameLength} characters.");
        }

        return trimmed;
    }

    private static VendorType RequireType(VendorType? type)
    {
        if (!type.HasValue)
        {
            throw new InvalidRequestException("Vendor type is required.");
        }

        ValidateType(type.Value);
        return type.Value;
    }

    private static void ValidateType(VendorType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new InvalidRequestException("Vendor type must be Supplier, Customer, or Both.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        if (await vendorRepository.NameExistsAsync(name, excludeId, cancellationToken))
        {
            throw new ConflictException($"A vendor named '{name}' already exists.");
        }
    }

    private static VendorResponse ToResponse(Vendor vendor) => new()
    {
        Id = vendor.Id,
        Name = vendor.Name,
        Type = vendor.Type,
        IsActive = vendor.IsActive
    };
}