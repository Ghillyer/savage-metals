using Dapper;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class LocationRepository(IDbConnectionFactory connectionFactory) : ILocationRepository
{
    private const string SelectColumns = $"""
        {nameof(Location.Id)},
        {nameof(Location.Name)},
        {nameof(Location.IsActive)}
        """;

    public async Task<IReadOnlyList<Location>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Locations
            WHERE @IncludeInactive = 1
               OR {nameof(Location.IsActive)} = 1
            ORDER BY {nameof(Location.Name)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { IncludeInactive = includeInactive }, cancellationToken: cancellationToken);
        var locations = await connection.QueryAsync<Location>(command);
        return locations.AsList();
    }

    public async Task<Location?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Locations
            WHERE {nameof(Location.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Location>(command);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Locations
                WHERE {nameof(Location.Name)} = @Name
                  AND (@ExcludeId IS NULL OR {nameof(Location.Id)} <> @ExcludeId)
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int> CreateAsync(Location location, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            INSERT INTO Locations (
                {nameof(Location.Name)},
                {nameof(Location.IsActive)}
            )
            OUTPUT INSERTED.{nameof(Location.Id)}
            VALUES (
                @Name,
                @IsActive
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, location, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            UPDATE Locations
            SET {nameof(Location.Name)} = @Name,
                {nameof(Location.IsActive)} = @IsActive
            WHERE {nameof(Location.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, location, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}