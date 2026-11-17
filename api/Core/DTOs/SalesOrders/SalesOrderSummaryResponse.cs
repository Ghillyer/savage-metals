namespace SavageMetals.Api.Core.DTOs.SalesOrders;

public record SalesOrderSummaryResponse
{
    public int Id { get; init; }
    public string SoNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string ContainerNumber { get; init; } = string.Empty;
    public int ItemCount { get; init; }
    public decimal TotalWeight { get; init; }
    public decimal Revenue { get; init; }
    public decimal Cost { get; init; }
    public decimal Profit { get; init; }
    public decimal Margin { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}