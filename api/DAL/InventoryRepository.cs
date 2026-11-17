using Dapper;
using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class InventoryRepository(IDbConnectionFactory connectionFactory) : IInventoryRepository
{
    private const string SelectColumns = $"""
        {nameof(InventoryItem.Id)},
        {nameof(InventoryItem.TagNumber)},
        {nameof(InventoryItem.ProductId)},
        {nameof(InventoryItem.LocationId)},
        {nameof(InventoryItem.Weight)},
        {nameof(InventoryItem.TotalCost)},
        {nameof(InventoryItem.Status)},
        {nameof(InventoryItem.CreatedAtUtc)},
        {nameof(InventoryItem.PurchaseOrderLineId)},
        {nameof(InventoryItem.SourceWorkOrderId)}
        """;
    public async Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(InventoryStatus? status, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ii.Id,
                ii.TagNumber,
                ii.ProductId,
                p.Name AS ProductName,
                p.IsScrap,
                ii.LocationId,
                l.Name AS LocationName,
                ii.Weight,
                CAST(ii.TotalCost AS decimal(18, 2)) AS TotalCost,
                ii.Status,
                ii.CreatedAtUtc
            FROM InventoryItems ii
            INNER JOIN Products p ON p.Id = ii.ProductId
            INNER JOIN Locations l ON l.Id = ii.LocationId
            WHERE @Status IS NULL OR ii.Status = @Status
            ORDER BY ii.TagNumber
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Status = status }, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<InventoryItemResponse>(command);
        return results.ToList();
    }

    public async Task<InventoryItem?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT
                {SelectColumns}
            FROM InventoryItems
            WHERE {nameof(InventoryItem.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<InventoryItem>(command);
    }

    public async Task<IReadOnlyList<InventoryItem>> GetByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT
                {SelectColumns}
            FROM InventoryItems
            WHERE {nameof(InventoryItem.Id)} IN @Ids
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Ids = ids }, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<InventoryItem>(command);
        return results.ToList();
    }

    public async Task<string?> FindExistingTagNumberAsync(IReadOnlyList<string> tagNumbers, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT TOP 1
                {nameof(InventoryItem.TagNumber)}
            FROM InventoryItems
            WHERE {nameof(InventoryItem.TagNumber)} IN @TagNumbers
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { TagNumbers = tagNumbers }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<string>(command);
    }
}