using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record PurchaseOrderSummaryResponse
{
    public int Id { get; init; }
    public string PoNumber { get; init; } = string.Empty;
    public string VendorName { get; init; } = string.Empty;
    public PurchaseOrderStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public decimal Total { get; init; }
}