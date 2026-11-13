using SavageMetals.Api.Core.DTOs.Vendors;
using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.Interfaces;

public interface IVendorService
{
    Task<IReadOnlyList<VendorResponse>> GetAllAsync(bool includeInactive, VendorType? type, CancellationToken cancellationToken = default);
    Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VendorResponse> CreateAsync(VendorRequest request, CancellationToken cancellationToken = default);
    Task<VendorResponse> UpdateAsync(int id, VendorRequest request, CancellationToken cancellationToken = default);
}