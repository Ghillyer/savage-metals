using Dapper;
using SavageMetals.Api.Core.DTOs.SalesOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class SalesOrderRepository(IDbConnectionFactory connectionFactory) : ISalesOrderRepository
{
    public async Task<IReadOnlyList<SalesOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                so.Id,
                so.SoNumber,
                c.Name AS CustomerName,
                so.ContainerNumber,
                totals.ItemCount,
                totals.TotalWeight,
                CAST(totals.Revenue AS decimal(18, 2)) AS Revenue,
                CAST(totals.Cost AS decimal(18, 2)) AS Cost,
                CAST(totals.Revenue - totals.Cost AS decimal(18, 2)) AS Profit,
                CAST(CASE WHEN totals.Revenue = 0 THEN 0 ELSE (totals.Revenue - totals.Cost) / totals.Revenue END AS decimal(9, 4)) AS Margin,
                so.CreatedAtUtc
            FROM SalesOrders so
            INNER JOIN Vendors c ON c.Id = so.CustomerId
            CROSS APPLY (
                SELECT
                    COUNT(*) AS ItemCount,
                    SUM(ii.Weight) AS TotalWeight,
                    SUM(ROUND(ii.Weight * sol.UnitPrice, 2)) AS Revenue,
                    SUM(ii.TotalCost) AS Cost
                FROM SalesOrderLines sol
                INNER JOIN InventoryItems ii ON ii.Id = sol.InventoryItemId
                WHERE sol.SalesOrderId = so.Id
            ) totals
            ORDER BY so.CreatedAtUtc DESC
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<SalesOrderSummaryResponse>(command);
        return results.ToList();
    }

    public async Task<SalesOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                so.Id,
                so.SoNumber,
                so.CustomerId,
                c.Name AS CustomerName,
                so.ContainerNumber,
                so.CreatedAtUtc
            FROM SalesOrders so
            INNER JOIN Vendors c ON c.Id = so.CustomerId
            WHERE so.Id = @Id;

            SELECT
                sol.Id,
                sol.InventoryItemId,
                ii.TagNumber,
                p.Name AS ProductName,
                ii.Weight,
                sol.UnitPrice,
                CAST(ROUND(ii.Weight * sol.UnitPrice, 2) AS decimal(18, 2)) AS Revenue,
                CAST(ii.TotalCost AS decimal(18, 2)) AS Cost,
                CAST(ROUND(ii.Weight * sol.UnitPrice, 2) - ii.TotalCost AS decimal(18, 2)) AS Profit
            FROM SalesOrderLines sol
            INNER JOIN InventoryItems ii ON ii.Id = sol.InventoryItemId
            INNER JOIN Products p ON p.Id = ii.ProductId
            WHERE sol.SalesOrderId = @Id
            ORDER BY ii.TagNumber;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var results = await connection.QueryMultipleAsync(command);

        var salesOrder = await results.ReadSingleOrDefaultAsync<SalesOrderResponse>();

        if (salesOrder is null)
        {
            return null;
        }

        var lines = (await results.ReadAsync<SalesOrderLineResponse>()).ToList();
        var revenue = lines.Sum(line => line.Revenue);
        var cost = lines.Sum(line => line.Cost);
        var profit = revenue - cost;

        return salesOrder with
        {
            Lines = lines,
            TotalWeight = lines.Sum(line => line.Weight),
            Revenue = revenue,
            Cost = cost,
            Profit = profit,
            Margin = revenue == 0 ? 0 : Math.Round(profit / revenue, 4, MidpointRounding.AwayFromZero),
        };
    }

    public async Task<bool> SoNumberExistsAsync(string soNumber, CancellationToken cancellationToken)
    {
        const string sql = $"""
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM SalesOrders
                WHERE {nameof(SalesOrder.SoNumber)} = @SoNumber
            ) THEN 1 ELSE 0 END
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { SoNumber = soNumber }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<int?> CreateAsync(SalesOrder salesOrder, IReadOnlyList<SalesOrderLine> lines, CancellationToken cancellationToken)
    {
        const string markSoldSql = $"""
            UPDATE InventoryItems
            SET {nameof(InventoryItem.Status)} = @SoldStatus
            WHERE {nameof(InventoryItem.Id)} IN @Ids
                AND {nameof(InventoryItem.Status)} = @AvailableStatus
            """;

        const string insertSalesOrderSql = $"""
            INSERT INTO SalesOrders (
                {nameof(SalesOrder.SoNumber)},
                {nameof(SalesOrder.CustomerId)},
                {nameof(SalesOrder.ContainerNumber)},
                {nameof(SalesOrder.CreatedAtUtc)},
                {nameof(SalesOrder.CreatedByUserId)}
            )
            OUTPUT INSERTED.{nameof(SalesOrder.Id)}
            VALUES (
                @{nameof(SalesOrder.SoNumber)},
                @{nameof(SalesOrder.CustomerId)},
                @{nameof(SalesOrder.ContainerNumber)},
                @{nameof(SalesOrder.CreatedAtUtc)},
                @{nameof(SalesOrder.CreatedByUserId)}
            )
            """;

        const string insertLineSql = $"""
            INSERT INTO SalesOrderLines (
                {nameof(SalesOrderLine.SalesOrderId)},
                {nameof(SalesOrderLine.InventoryItemId)},
                {nameof(SalesOrderLine.UnitPrice)}
            )
            VALUES (
                @{nameof(SalesOrderLine.SalesOrderId)},
                @{nameof(SalesOrderLine.InventoryItemId)},
                @{nameof(SalesOrderLine.UnitPrice)}
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var markSoldCommand = new CommandDefinition(
            markSoldSql,
            new
            {
                Ids = lines.Select(line => line.InventoryItemId).ToList(),
                SoldStatus = InventoryStatus.Sold,
                AvailableStatus = InventoryStatus.Available,
            },
            transaction,
            cancellationToken: cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(markSoldCommand);

        if (rowsAffected != lines.Count)
        {
            return null;
        }

        var insertSalesOrderCommand = new CommandDefinition(insertSalesOrderSql, salesOrder, transaction, cancellationToken: cancellationToken);
        var id = await connection.QuerySingleAsync<int>(insertSalesOrderCommand);

        foreach (var line in lines)
        {
            line.SalesOrderId = id;
        }

        var insertLineCommand = new CommandDefinition(insertLineSql, lines, transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(insertLineCommand);

        await transaction.CommitAsync(cancellationToken);

        return id;
    }
}