namespace SavageMetals.Api.Core.DTOs.SalesOrders;

public record SalesOrderLineResponse
{
    public int Id { get; init; }
    public int InventoryItemId { get; init; }
    public string TagNumber { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Revenue { get; init; }
    public decimal Cost { get; init; }
    public decimal Profit { get; init; }
}