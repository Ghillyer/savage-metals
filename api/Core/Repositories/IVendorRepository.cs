using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IVendorRepository
{
    Task<IReadOnlyList<Vendor>> GetAllAsync(bool includeInactive, VendorType? type, CancellationToken cancellationToken = default);
    Task<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Vendor vendor, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default);
}