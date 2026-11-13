using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class PurchaseOrderLine
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public MaterialGrade Grade { get; set; }
    public decimal Weight { get; set; }
    public decimal UnitPrice { get; set; }
}