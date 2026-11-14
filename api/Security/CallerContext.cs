using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;

namespace SavageMetals.Api.Core;

public class CallerContext : ICallerContext
{
    public int UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    public void Initialize(AppUser user)
    {
        UserId = user.Id;
        Email = user.Email;
        DisplayName = user.DisplayName;
        Role = user.Role;
    }
}