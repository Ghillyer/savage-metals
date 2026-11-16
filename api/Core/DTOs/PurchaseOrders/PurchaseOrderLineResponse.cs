using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record PurchaseOrderLineResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public MaterialGrade Grade { get; init; }
    public decimal Weight { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
}