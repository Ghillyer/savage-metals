namespace SavageMetals.Api.Models;

public class SalesOrderLine
{
    public int Id { get; set; }
    public int SalesOrderId { get; set; }
    public int InventoryItemId { get; set; }
    public decimal UnitPrice { get; set; }
}