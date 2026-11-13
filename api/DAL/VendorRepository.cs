using Dapper;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class VendorRepository(IDbConnectionFactory connectionFactory) : IVendorRepository
{
    private const string SelectColumns = $"""
        {nameof(Vendor.Id)},
        {nameof(Vendor.Name)},
        {nameof(Vendor.Type)},
        {nameof(Vendor.IsActive)}
        """;

    public async Task<IReadOnlyList<Vendor>> GetAllAsync(bool includeInactive, VendorType? type, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Vendors
            WHERE (@IncludeInactive = 1 OR {nameof(Vendor.IsActive)} = 1)
              AND (@Type IS NULL OR {nameof(Vendor.Type)} = @Type OR {nameof(Vendor.Type)} = @Both)
            ORDER BY {nameof(Vendor.Name)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var parameters = new { IncludeInactive = includeInactive, Type = type, Both = VendorType.Both };
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var vendors = await connection.QueryAsync<Vendor>(command);
        return vendors.AsList();
    }

    public async Task<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Vendors
            WHERE {nameof(Vendor.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Vendor>(command);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Vendors
                WHERE {nameof(Vendor.Name)} = @Name
                  AND (@ExcludeId IS NULL OR {nameof(Vendor.Id)} <> @ExcludeId)
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int> CreateAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            INSERT INTO Vendors (
                {nameof(Vendor.Name)},
                {nameof(Vendor.Type)},
                {nameof(Vendor.IsActive)}
            )
            OUTPUT INSERTED.{nameof(Vendor.Id)}
            VALUES (
                @Name,
                @Type,
                @IsActive
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, vendor, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task UpdateAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            UPDATE Vendors
            SET {nameof(Vendor.Name)} = @Name,
                {nameof(Vendor.Type)} = @Type,
                {nameof(Vendor.IsActive)} = @IsActive
            WHERE {nameof(Vendor.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, vendor, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}