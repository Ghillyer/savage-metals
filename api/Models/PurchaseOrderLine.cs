using SavageMetals.Api.Enums;

namespace SavageMetals.Api.Models;

public class PurchaseOrderLine
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int ProductId { get; set; }
    public MaterialGrade Grade { get; set; }
    public decimal Weight { get; set; }
    public decimal UnitPrice { get; set; }
}