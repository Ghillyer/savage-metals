using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.DTOs.Users;

public record UpdateUserRequest
{
    public string DisplayName { get; init; } = string.Empty;
    public UserRole? Role { get; init; }
    public bool IsActive { get; init; }
}