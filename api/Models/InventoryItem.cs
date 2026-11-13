using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public string TagNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;
    public decimal Weight { get; set; }
    public decimal TotalCost { get; set; }
    public InventoryStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? PurchaseOrderLineId { get; set; }
    public PurchaseOrderLine? PurchaseOrderLine { get; set; }
    public int? SourceWorkOrderId { get; set; }
    public WorkOrder? SourceWorkOrder { get; set; }
}