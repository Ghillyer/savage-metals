using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.WorkOrders;

public record WorkOrderOutputResponse
{
    public int Id { get; init; }
    public string TagNumber { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public bool IsScrap { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public decimal TotalCost { get; init; }
    public InventoryStatus Status { get; init; }
}