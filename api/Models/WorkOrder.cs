namespace SavageMetals.Api.Models;

public class WorkOrder
{
    public int Id { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public int InputItemId { get; set; }
    public InventoryItem InputItem { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
    public List<InventoryItem> Outputs { get; set; } = [];
}