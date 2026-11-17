namespace SavageMetals.Api.Core.DTOs.SalesOrders;

public record SalesOrderLineRequest
{
    public int InventoryItemId { get; init; }
    public decimal UnitPrice { get; init; }
}