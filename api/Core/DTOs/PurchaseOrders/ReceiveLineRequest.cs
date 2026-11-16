namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record ReceiveLineRequest
{
    public int PurchaseOrderLineId { get; init; }
    public string TagNumber { get; init; } = string.Empty;
    public int LocationId { get; init; }
}