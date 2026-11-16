namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record PurchaseOrderRequest
{
    public string PoNumber { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public IReadOnlyList<PurchaseOrderLineRequest> Lines { get; init; } = [];
}