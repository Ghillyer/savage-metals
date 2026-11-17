using FirebaseAdmin.Auth;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Security;

public class FirebaseIdentityService(FirebaseAuth firebaseAuth) : IIdentityService
{
    public async Task<string> CreateUserAsync(string email, string displayName, CancellationToken cancellationToken)
    {
        try
        {
            var user = await firebaseAuth.CreateUserAsync(new UserRecordArgs { Email = email, DisplayName = displayName }, cancellationToken);
            return user.Uid;
        }
        catch (FirebaseAuthException exception) when (exception.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
        {
            throw new ConflictException($"A sign-in account for {email} already exists.");
        }
    }

    public async Task DeleteUserAsync(string uid, CancellationToken cancellationToken)
    {
        await firebaseAuth.DeleteUserAsync(uid, cancellationToken);
    }
}