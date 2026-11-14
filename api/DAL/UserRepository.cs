using Dapper;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class UserRepository(IDbConnectionFactory connectionFactory) : IUserRepository
{
    private const string SelectColumns = $"""
        {nameof(AppUser.Id)},
        {nameof(AppUser.FirebaseUid)},
        {nameof(AppUser.Email)},
        {nameof(AppUser.DisplayName)},
        {nameof(AppUser.Role)},
        {nameof(AppUser.IsActive)}
        """;

    public async Task<AppUser?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Users
            WHERE {nameof(AppUser.FirebaseUid)} = @FirebaseUid
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { FirebaseUid = firebaseUid }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<AppUser>(command);
    }
}