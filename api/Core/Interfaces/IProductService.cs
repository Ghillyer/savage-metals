using SavageMetals.Api.Core.DTOs.Products;

namespace SavageMetals.Api.Core.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default);
    Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default);
    Task<ProductResponse> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default);
}