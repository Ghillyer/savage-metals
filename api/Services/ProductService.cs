using SavageMetals.Api.Core.DTOs.Products;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class ProductService(IProductRepository productRepository) : IProductService
{
    private const int MaxNameLength = 200;

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        var products = await productRepository.GetAllAsync(includeInactive, cancellationToken);
        return products.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product {id} was not found.");

        return ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var name = NormalizeName(request.Name);
        ValidatePrices(request.PurchasePrice, request.SalesPrice);
        await EnsureNameIsUniqueAsync(name, null, cancellationToken);

        var product = new Product
        {
            Name = name,
            PurchasePrice = request.PurchasePrice,
            SalesPrice = request.SalesPrice,
            IsScrap = request.IsScrap,
            IsActive = true
        };

        product.Id = await productRepository.CreateAsync(product, cancellationToken);
        return ToResponse(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await productRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product {id} was not found.");

        var name = NormalizeName(request.Name);
        ValidatePrices(request.PurchasePrice, request.SalesPrice);
        await EnsureNameIsUniqueAsync(name, id, cancellationToken);

        product.Name = name;
        product.PurchasePrice = request.PurchasePrice;
        product.SalesPrice = request.SalesPrice;
        product.IsScrap = request.IsScrap;
        product.IsActive = request.IsActive;

        await productRepository.UpdateAsync(product, cancellationToken);
        return ToResponse(product);
    }

    private static string NormalizeName(string? name)
    {
        var trimmed = name?.Trim() ?? string.Empty;

        if (trimmed.Length == 0)
        {
            throw new InvalidRequestException("Product name is required.");
        }

        if (trimmed.Length > MaxNameLength)
        {
            throw new InvalidRequestException($"Product name cannot exceed {MaxNameLength} characters.");
        }

        return trimmed;
    }

    private static void ValidatePrices(decimal? purchasePrice, decimal? salesPrice)
    {
        if (purchasePrice < 0 || salesPrice < 0)
        {
            throw new InvalidRequestException("Prices cannot be negative.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludeId, CancellationToken cancellationToken)
    {
        if (await productRepository.NameExistsAsync(name, excludeId, cancellationToken))
        {
            throw new ConflictException($"A product named '{name}' already exists.");
        }
    }

    private static ProductResponse ToResponse(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        PurchasePrice = product.PurchasePrice,
        SalesPrice = product.SalesPrice,
        IsScrap = product.IsScrap,
        IsActive = product.IsActive
    };
}