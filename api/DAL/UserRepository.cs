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

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Users
            ORDER BY {nameof(AppUser.DisplayName)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var users = await connection.QueryAsync<AppUser>(command);
        return users.AsList();
    }

    public async Task<AppUser?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Users
            WHERE {nameof(AppUser.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<AppUser>(command);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Users
                WHERE {nameof(AppUser.Email)} = @Email
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int> CreateAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            INSERT INTO Users (
                {nameof(AppUser.FirebaseUid)},
                {nameof(AppUser.Email)},
                {nameof(AppUser.DisplayName)},
                {nameof(AppUser.Role)},
                {nameof(AppUser.IsActive)}
            )
            OUTPUT INSERTED.{nameof(AppUser.Id)}
            VALUES (
                @{nameof(AppUser.FirebaseUid)},
                @{nameof(AppUser.Email)},
                @{nameof(AppUser.DisplayName)},
                @{nameof(AppUser.Role)},
                @{nameof(AppUser.IsActive)}
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, user, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            UPDATE Users
            SET {nameof(AppUser.DisplayName)} = @{nameof(AppUser.DisplayName)},
                {nameof(AppUser.Role)} = @{nameof(AppUser.Role)},
                {nameof(AppUser.IsActive)} = @{nameof(AppUser.IsActive)}
            WHERE {nameof(AppUser.Id)} = @{nameof(AppUser.Id)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, user, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }

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