namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record ReceivePurchaseOrderRequest
{
    public IReadOnlyList<ReceiveLineRequest> Lines { get; init; } = [];
}