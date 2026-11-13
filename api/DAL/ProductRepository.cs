using Dapper;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class ProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
{
    private const string SelectColumns = $"""
        {nameof(Product.Id)},
        {nameof(Product.Name)},
        {nameof(Product.PurchasePrice)},
        {nameof(Product.SalesPrice)},
        {nameof(Product.IsScrap)},
        {nameof(Product.IsActive)}
        """;

    public async Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Products
            WHERE @IncludeInactive = 1
               OR {nameof(Product.IsActive)} = 1
            ORDER BY {nameof(Product.Name)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { IncludeInactive = includeInactive }, cancellationToken: cancellationToken);
        var products = await connection.QueryAsync<Product>(command);
        return products.AsList();
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT {SelectColumns}
            FROM Products
            WHERE {nameof(Product.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Product>(command);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Products
                WHERE {nameof(Product.Name)} = @Name
                  AND (@ExcludeId IS NULL OR {nameof(Product.Id)} <> @ExcludeId)
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Name = name, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            INSERT INTO Products (
                {nameof(Product.Name)},
                {nameof(Product.PurchasePrice)},
                {nameof(Product.SalesPrice)},
                {nameof(Product.IsScrap)},
                {nameof(Product.IsActive)}
            )
            OUTPUT INSERTED.{nameof(Product.Id)}
            VALUES (
                @Name,
                @PurchasePrice,
                @SalesPrice,
                @IsScrap,
                @IsActive
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, product, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int>(command);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = $"""
            UPDATE Products
            SET {nameof(Product.Name)} = @Name,
                {nameof(Product.PurchasePrice)} = @PurchasePrice,
                {nameof(Product.SalesPrice)} = @SalesPrice,
                {nameof(Product.IsScrap)} = @IsScrap,
                {nameof(Product.IsActive)} = @IsActive
            WHERE {nameof(Product.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, product, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}