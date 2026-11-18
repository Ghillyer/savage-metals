namespace SavageMetals.Api.Core.DTOs.PurchaseOrders;

public record NextPurchaseOrderNumberResponse
{
    public string PoNumber { get; init; } = string.Empty;
}