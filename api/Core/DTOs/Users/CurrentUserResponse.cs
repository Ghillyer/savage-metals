using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.Users;

public record CurrentUserResponse
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
}