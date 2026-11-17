using System.Net.Mail;
using SavageMetals.Api.Core.DTOs.Users;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Exceptions;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class UserService(
    IUserRepository userRepository,
    IIdentityService identityService,
    ICallerContext callerContext) : IUserService
{
    private const int MaxLength = 200;

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(ToResponse).ToList();
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        var displayName = NormalizeRequired(request.DisplayName, "Name");
        var role = ValidateRole(request.Role);

        if (await userRepository.EmailExistsAsync(email, cancellationToken))
        {
            throw new ConflictException($"A user with email {email} already exists.");
        }

        var firebaseUid = await identityService.CreateUserAsync(email, displayName, cancellationToken);

        var user = new AppUser
        {
            FirebaseUid = firebaseUid,
            Email = email,
            DisplayName = displayName,
            Role = role,
            IsActive = true,
        };

        try
        {
            user.Id = await userRepository.CreateAsync(user, cancellationToken);
        }
        catch
        {
            await identityService.DeleteUserAsync(firebaseUid, CancellationToken.None);
            throw;
        }

        return ToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"User {id} was not found.");

        var displayName = NormalizeRequired(request.DisplayName, "Name");
        var role = ValidateRole(request.Role);

        if (user.Id == callerContext.UserId && (role != UserRole.Admin || !request.IsActive))
        {
            throw new InvalidRequestException("You can't remove your own admin access or deactivate your own account.");
        }

        user.DisplayName = displayName;
        user.Role = role;
        user.IsActive = request.IsActive;

        await userRepository.UpdateAsync(user, cancellationToken);

        return ToResponse(user);
    }

    private static UserResponse ToResponse(AppUser user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            IsActive = user.IsActive,
        };
    }

    private static UserRole ValidateRole(UserRole? role)
    {
        if (role is not { } value || !Enum.IsDefined(value))
        {
            throw new InvalidRequestException("Choose a role.");
        }

        return value;
    }

    private static string NormalizeEmail(string? value)
    {
        var email = NormalizeRequired(value, "Email").ToLowerInvariant();

        if (!MailAddress.TryCreate(email, out var address) || address.Address != email)
        {
            throw new InvalidRequestException("Enter a valid email address.");
        }

        return email;
    }

    private static string NormalizeRequired(string? value, string fieldName)
    {
        var trimmed = value?.Trim();

        if (string.IsNullOrEmpty(trimmed))
        {
            throw new InvalidRequestException($"{fieldName} is required.");
        }

        if (trimmed.Length > MaxLength)
        {
            throw new InvalidRequestException($"{fieldName} must be {MaxLength} characters or fewer.");
        }

        return trimmed;
    }
}