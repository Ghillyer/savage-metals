namespace SavageMetals.Api.Core.DTOs.Reports;

public record InventoryReportResponse
{
    public int ItemCount { get; init; }
    public decimal TotalWeight { get; init; }
    public decimal TotalValue { get; init; }
    public IReadOnlyList<InventoryReportRowResponse> ByProduct { get; init; } = [];
    public IReadOnlyList<InventoryReportRowResponse> ByLocation { get; init; } = [];
}