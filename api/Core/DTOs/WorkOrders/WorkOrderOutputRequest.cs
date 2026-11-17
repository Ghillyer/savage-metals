namespace SavageMetals.Api.Core.DTOs.WorkOrders;

public record WorkOrderOutputRequest
{
    public string TagNumber { get; init; } = string.Empty;
    public int ProductId { get; init; }
    public int LocationId { get; init; }
    public decimal Weight { get; init; }
}