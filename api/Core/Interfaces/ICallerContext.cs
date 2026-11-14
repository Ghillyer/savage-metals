using SavageMetals.Api.Core.Enums;

namespace SavageMetals.Api.Core.Interfaces;

public interface ICallerContext
{
    int UserId { get; }
    string Email { get; }
    string DisplayName { get; }
    UserRole Role { get; }
}