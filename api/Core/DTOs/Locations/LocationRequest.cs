namespace SavageMetals.Api.Core.DTOs.Locations;

public record LocationRequest
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}