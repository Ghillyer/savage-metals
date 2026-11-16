using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record PurchaseOrderLineRequest
{
    public int ProductId { get; init; }
    public MaterialGrade? Grade { get; init; }
    public decimal Weight { get; init; }
    public decimal UnitPrice { get; init; }
}