using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.Inventory;

public record InventoryItemResponse
{
    public int Id { get; init; }
    public string TagNumber { get; init; } = string.Empty;
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public bool IsScrap { get; init; }
    public int LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public decimal Weight { get; init; }
    public decimal TotalCost { get; init; }
    public InventoryStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}