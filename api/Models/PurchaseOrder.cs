using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class PurchaseOrder
{
    public int Id { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;
    public PurchaseOrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
    public List<PurchaseOrderLine> Lines { get; set; } = [];
}