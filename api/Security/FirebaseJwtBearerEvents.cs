using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Security;

public class FirebaseJwtBearerEvents(IUserRepository userRepository, CallerContext callerContext) : JwtBearerEvents
{
    private const string FirebaseUidClaim = "user_id";

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var firebaseUid = context.Principal?.FindFirstValue(FirebaseUidClaim);

        if (string.IsNullOrEmpty(firebaseUid))
        {
            context.Fail("Token does not contain a Firebase user id.");
            return;
        }

        var user = await userRepository.GetByFirebaseUidAsync(firebaseUid, context.HttpContext.RequestAborted);

        if (user is null || !user.IsActive)
        {
            context.Fail("User is not registered or is inactive.");
            return;
        }

        callerContext.Initialize(user);

        if (context.Principal?.Identity is ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, user.Role.ToString()));
        }
    }
}