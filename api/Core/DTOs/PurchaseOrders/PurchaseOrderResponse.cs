using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record PurchaseOrderResponse
{
    public int Id { get; init; }
    public string PoNumber { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public PurchaseOrderStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ReceivedAtUtc { get; init; }
    public decimal Total { get; init; }
    public IReadOnlyList<PurchaseOrderLineResponse> Lines { get; init; } = [];
}