using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}