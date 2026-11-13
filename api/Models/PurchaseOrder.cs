using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class PurchaseOrder
{
    public int Id { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public int CreatedByUserId { get; set; }
}