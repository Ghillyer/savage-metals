namespace SavageMetals.Api.Core.DTOs.WorkOrders;

public record WorkOrderResponse
{
    public int Id { get; init; }
    public string WorkOrderNumber { get; init; } = string.Empty;
    public int InputItemId { get; init; }
    public string InputTagNumber { get; init; } = string.Empty;
    public string InputProductName { get; init; } = string.Empty;
    public decimal InputWeight { get; init; }
    public decimal InputCost { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public IReadOnlyList<WorkOrderOutputResponse> Outputs { get; init; } = [];
}