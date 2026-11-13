namespace SavageMetals.Api.Core.DTOs.Products;

public record ProductResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal? PurchasePrice { get; init; }
    public decimal? SalesPrice { get; init; }
    public bool IsScrap { get; init; }
    public bool IsActive { get; init; }
}