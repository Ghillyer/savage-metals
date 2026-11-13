namespace SavageMetals.Api.Core.DTOs.Locations;

public record LocationResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}