using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.Vendors;

public record VendorRequest
{
    public string Name { get; init; } = string.Empty;
    public VendorType? Type { get; init; }
    public bool IsActive { get; init; }
}