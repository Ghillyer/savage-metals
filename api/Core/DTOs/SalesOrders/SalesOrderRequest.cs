namespace SavageMetals.Api.Core.DTOs.SalesOrders;

public record SalesOrderRequest
{
    public string SoNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string ContainerNumber { get; init; } = string.Empty;
    public IReadOnlyList<SalesOrderLineRequest> Lines { get; init; } = [];
}