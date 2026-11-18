using System.Data.Common;
using Dapper;
using SavageMetals.Api.Core.DTOs.PurchaseOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class PurchaseOrderRepository(IDbConnectionFactory connectionFactory) : IPurchaseOrderRepository
{
    private const string SelectColumns = $"""
        {nameof(PurchaseOrder.Id)},
        {nameof(PurchaseOrder.PoNumber)},
        {nameof(PurchaseOrder.VendorId)},
        {nameof(PurchaseOrder.Status)},
        {nameof(PurchaseOrder.CreatedAtUtc)},
        {nameof(PurchaseOrder.ReceivedAtUtc)},
        {nameof(PurchaseOrder.CreatedByUserId)}
        """;

    private const string LineColumns = $"""
        {nameof(PurchaseOrderLine.Id)},
        {nameof(PurchaseOrderLine.PurchaseOrderId)},
        {nameof(PurchaseOrderLine.ProductId)},
        {nameof(PurchaseOrderLine.Grade)},
        {nameof(PurchaseOrderLine.Weight)},
        {nameof(PurchaseOrderLine.UnitPrice)}
        """;

    private const string InsertLineSql = $"""
        INSERT INTO PurchaseOrderLines (
            {nameof(PurchaseOrderLine.PurchaseOrderId)},
            {nameof(PurchaseOrderLine.ProductId)},
            {nameof(PurchaseOrderLine.Grade)},
            {nameof(PurchaseOrderLine.Weight)},
            {nameof(PurchaseOrderLine.UnitPrice)}
        )
        VALUES (
            @{nameof(PurchaseOrderLine.PurchaseOrderId)},
            @{nameof(PurchaseOrderLine.ProductId)},
            @{nameof(PurchaseOrderLine.Grade)},
            @{nameof(PurchaseOrderLine.Weight)},
            @{nameof(PurchaseOrderLine.UnitPrice)}
        )
        """;

    public async Task<IReadOnlyList<PurchaseOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                po.Id,
                po.PoNumber,
                v.Name AS VendorName,
                po.Status,
                po.CreatedAtUtc,
                CAST(COALESCE(totals.Total, 0) AS decimal(18, 2)) AS Total
            FROM PurchaseOrders po
            INNER JOIN Vendors v ON v.Id = po.VendorId
            OUTER APPLY (
                SELECT SUM(ROUND(pol.Weight * pol.UnitPrice, 2)) AS Total
                FROM PurchaseOrderLines pol
                WHERE pol.PurchaseOrderId = po.Id
            ) totals
            ORDER BY po.CreatedAtUtc DESC
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<PurchaseOrderSummaryResponse>(command);
        return results.ToList();
    }

    public async Task<PurchaseOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                po.Id,
                po.PoNumber,
                po.VendorId,
                v.Name AS VendorName,
                po.Status,
                po.CreatedAtUtc,
                po.ReceivedAtUtc
            FROM PurchaseOrders po
            INNER JOIN Vendors v ON v.Id = po.VendorId
            WHERE po.Id = @Id;

            SELECT
                pol.Id,
                pol.ProductId,
                p.Name AS ProductName,
                pol.Grade,
                pol.Weight,
                pol.UnitPrice,
                CAST(ROUND(pol.Weight * pol.UnitPrice, 2) AS decimal(18, 2)) AS LineTotal
            FROM PurchaseOrderLines pol
            INNER JOIN Products p ON p.Id = pol.ProductId
            WHERE pol.PurchaseOrderId = @Id
            ORDER BY pol.Id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var results = await connection.QueryMultipleAsync(command);

        var purchaseOrder = await results.ReadSingleOrDefaultAsync<PurchaseOrderResponse>();

        if (purchaseOrder is null)
        {
            return null;
        }

        var lines = (await results.ReadAsync<PurchaseOrderLineResponse>()).ToList();

        return purchaseOrder with
        {
            Lines = lines,
            Total = lines.Sum(line => line.LineTotal),
        };
    }

    public async Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT
                {SelectColumns}
            FROM PurchaseOrders
            WHERE {nameof(PurchaseOrder.Id)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<PurchaseOrder>(command);
    }

    public async Task<IReadOnlyList<PurchaseOrderLine>> GetLinesAsync(int purchaseOrderId, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT
                {LineColumns}
            FROM PurchaseOrderLines
            WHERE {nameof(PurchaseOrderLine.PurchaseOrderId)} = @PurchaseOrderId
            ORDER BY {nameof(PurchaseOrderLine.Id)}
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { PurchaseOrderId = purchaseOrderId }, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<PurchaseOrderLine>(command);
        return results.ToList();
    }

    public async Task<bool> PoNumberExistsAsync(string poNumber, int? excludeId, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM PurchaseOrders
                WHERE {nameof(PurchaseOrder.PoNumber)} = @PoNumber
                    AND (@ExcludeId IS NULL OR {nameof(PurchaseOrder.Id)} <> @ExcludeId)
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { PoNumber = poNumber, ExcludeId = excludeId }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int?> GetMaxNumericPoNumberAsync(CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT MAX(TRY_CAST({nameof(PurchaseOrder.PoNumber)} AS int))
            FROM PurchaseOrders
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<int?>(command);
    }

    public async Task<int> CreateAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderLine> lines, CancellationToken cancellationToken)
    {
        const string sql = $"""
            INSERT INTO PurchaseOrders (
                {nameof(PurchaseOrder.PoNumber)},
                {nameof(PurchaseOrder.VendorId)},
                {nameof(PurchaseOrder.Status)},
                {nameof(PurchaseOrder.CreatedAtUtc)},
                {nameof(PurchaseOrder.CreatedByUserId)}
            )
            OUTPUT INSERTED.{nameof(PurchaseOrder.Id)}
            VALUES (
                @{nameof(PurchaseOrder.PoNumber)},
                @{nameof(PurchaseOrder.VendorId)},
                @{nameof(PurchaseOrder.Status)},
                @{nameof(PurchaseOrder.CreatedAtUtc)},
                @{nameof(PurchaseOrder.CreatedByUserId)}
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var command = new CommandDefinition(sql, purchaseOrder, transaction, cancellationToken: cancellationToken);
        var id = await connection.QuerySingleAsync<int>(command);

        await InsertLinesAsync(connection, transaction, id, lines, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return id;
    }

    public async Task<bool> UpdateAsync(PurchaseOrder purchaseOrder, IReadOnlyList<PurchaseOrderLine> lines, CancellationToken cancellationToken)
    {
        const string updateSql = $"""
            UPDATE PurchaseOrders
            SET
                {nameof(PurchaseOrder.PoNumber)} = @PoNumber,
                {nameof(PurchaseOrder.VendorId)} = @VendorId
            WHERE {nameof(PurchaseOrder.Id)} = @Id
                AND {nameof(PurchaseOrder.Status)} = @OpenStatus
            """;

        const string deleteLinesSql = $"""
            DELETE FROM PurchaseOrderLines
            WHERE {nameof(PurchaseOrderLine.PurchaseOrderId)} = @Id
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var updateCommand = new CommandDefinition(
            updateSql,
            new
            {
                purchaseOrder.Id,
                purchaseOrder.PoNumber,
                purchaseOrder.VendorId,
                OpenStatus = PurchaseOrderStatus.Open,
            },
            transaction,
            cancellationToken: cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(updateCommand);

        if (rowsAffected == 0)
        {
            return false;
        }

        var deleteCommand = new CommandDefinition(deleteLinesSql, new { purchaseOrder.Id }, transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(deleteCommand);

        await InsertLinesAsync(connection, transaction, purchaseOrder.Id, lines, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ReceiveAsync(int purchaseOrderId, DateTime receivedAtUtc, IReadOnlyList<InventoryItem> items, CancellationToken cancellationToken)
    {
        const string updateSql = $"""
            UPDATE PurchaseOrders
            SET
                {nameof(PurchaseOrder.Status)} = @ReceivedStatus,
                {nameof(PurchaseOrder.ReceivedAtUtc)} = @ReceivedAtUtc
            WHERE {nameof(PurchaseOrder.Id)} = @Id
                AND {nameof(PurchaseOrder.Status)} = @OpenStatus
            """;

        const string insertItemSql = $"""
            INSERT INTO InventoryItems (
                {nameof(InventoryItem.TagNumber)},
                {nameof(InventoryItem.ProductId)},
                {nameof(InventoryItem.LocationId)},
                {nameof(InventoryItem.Weight)},
                {nameof(InventoryItem.TotalCost)},
                {nameof(InventoryItem.Status)},
                {nameof(InventoryItem.CreatedAtUtc)},
                {nameof(InventoryItem.PurchaseOrderLineId)}
            )
            VALUES (
                @{nameof(InventoryItem.TagNumber)},
                @{nameof(InventoryItem.ProductId)},
                @{nameof(InventoryItem.LocationId)},
                @{nameof(InventoryItem.Weight)},
                @{nameof(InventoryItem.TotalCost)},
                @{nameof(InventoryItem.Status)},
                @{nameof(InventoryItem.CreatedAtUtc)},
                @{nameof(InventoryItem.PurchaseOrderLineId)}
            )
            """;

        const string selectLineIdsSql = $"""
            SELECT
                {nameof(PurchaseOrderLine.Id)}
            FROM PurchaseOrderLines
            WHERE {nameof(PurchaseOrderLine.PurchaseOrderId)} = @PurchaseOrderId
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var updateCommand = new CommandDefinition(
            updateSql,
            new
            {
                Id = purchaseOrderId,
                ReceivedAtUtc = receivedAtUtc,
                ReceivedStatus = PurchaseOrderStatus.Received,
                OpenStatus = PurchaseOrderStatus.Open,
            },
            transaction,
            cancellationToken: cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(updateCommand);

        if (rowsAffected == 0)
        {
            return false;
        }

        var lineIdsCommand = new CommandDefinition(
            selectLineIdsSql,
            new { PurchaseOrderId = purchaseOrderId },
            transaction,
            cancellationToken: cancellationToken);

        var currentLineIds = (await connection.QueryAsync<int>(lineIdsCommand)).Select(id => (int?)id).OrderBy(id => id).ToList();
        var receivedLineIds = items.Select(item => item.PurchaseOrderLineId).OrderBy(id => id).ToList();

        if (!currentLineIds.SequenceEqual(receivedLineIds))
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        var insertCommand = new CommandDefinition(insertItemSql, items, transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(insertCommand);

        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    private static async Task InsertLinesAsync(DbConnection connection, DbTransaction transaction, int purchaseOrderId, IReadOnlyList<PurchaseOrderLine> lines, CancellationToken cancellationToken)
    {
        foreach (var line in lines)
        {
            line.PurchaseOrderId = purchaseOrderId;
        }

        var command = new CommandDefinition(InsertLineSql, lines, transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}