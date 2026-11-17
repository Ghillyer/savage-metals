namespace SavageMetals.Api.Core.DTOs.Reports;

public record InventoryReportRowResponse
{
    public string Name { get; init; } = string.Empty;
    public int ItemCount { get; init; }
    public decimal TotalWeight { get; init; }
    public decimal TotalValue { get; init; }
}