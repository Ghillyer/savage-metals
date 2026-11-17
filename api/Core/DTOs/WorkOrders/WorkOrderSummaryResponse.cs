namespace SavageMetals.Api.Core.DTOs.WorkOrders;

public record WorkOrderSummaryResponse
{
    public int Id { get; init; }
    public string WorkOrderNumber { get; init; } = string.Empty;
    public string InputTagNumber { get; init; } = string.Empty;
    public string InputProductName { get; init; } = string.Empty;
    public decimal InputWeight { get; init; }
    public int OutputCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}