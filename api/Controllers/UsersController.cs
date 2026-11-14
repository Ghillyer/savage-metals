using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Users;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController(ICallerContext callerContext) : ControllerBase
{
    [HttpGet("me")]
    [ProducesResponseType<CurrentUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<CurrentUserResponse> GetCurrentUser()
    {
        var currentUser = new CurrentUserResponse
        {
            Id = callerContext.UserId,
            Email = callerContext.Email,
            DisplayName = callerContext.DisplayName,
            Role = callerContext.Role
        };

        return Ok(currentUser);
    }
}