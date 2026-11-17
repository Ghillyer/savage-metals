namespace SavageMetals.Api.Core.DTOs.WorkOrders;

public record WorkOrderRequest
{
    public int InputItemId { get; init; }
    public IReadOnlyList<WorkOrderOutputRequest> Outputs { get; init; } = [];
}