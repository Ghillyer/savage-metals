namespace SavageMetals.Api.Core.Interfaces;

public interface IIdentityService
{
    Task<string> CreateUserAsync(string email, string displayName, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string uid, CancellationToken cancellationToken = default);
}