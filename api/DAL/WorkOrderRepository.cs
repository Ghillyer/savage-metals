using Dapper;
using SavageMetals.Api.Core.DTOs.WorkOrders;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Models;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class WorkOrderRepository(IDbConnectionFactory connectionFactory) : IWorkOrderRepository
{
    public async Task<IReadOnlyList<WorkOrderSummaryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                wo.Id,
                wo.WorkOrderNumber,
                ii.TagNumber AS InputTagNumber,
                p.Name AS InputProductName,
                ii.Weight AS InputWeight,
                (SELECT COUNT(*) FROM InventoryItems o WHERE o.SourceWorkOrderId = wo.Id) AS OutputCount,
                wo.CreatedAtUtc
            FROM WorkOrders wo
            INNER JOIN InventoryItems ii ON ii.Id = wo.InputItemId
            INNER JOIN Products p ON p.Id = ii.ProductId
            ORDER BY wo.CreatedAtUtc DESC
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var results = await connection.QueryAsync<WorkOrderSummaryResponse>(command);
        return results.ToList();
    }

    public async Task<WorkOrderResponse?> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                wo.Id,
                wo.WorkOrderNumber,
                wo.InputItemId,
                ii.TagNumber AS InputTagNumber,
                p.Name AS InputProductName,
                ii.Weight AS InputWeight,
                CAST(ii.TotalCost AS decimal(18, 2)) AS InputCost,
                wo.CreatedAtUtc
            FROM WorkOrders wo
            INNER JOIN InventoryItems ii ON ii.Id = wo.InputItemId
            INNER JOIN Products p ON p.Id = ii.ProductId
            WHERE wo.Id = @Id;

            SELECT
                ii.Id,
                ii.TagNumber,
                p.Name AS ProductName,
                p.IsScrap,
                l.Name AS LocationName,
                ii.Weight,
                CAST(ii.TotalCost AS decimal(18, 2)) AS TotalCost,
                ii.Status
            FROM InventoryItems ii
            INNER JOIN Products p ON p.Id = ii.ProductId
            INNER JOIN Locations l ON l.Id = ii.LocationId
            WHERE ii.SourceWorkOrderId = @Id
            ORDER BY ii.Id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        using var results = await connection.QueryMultipleAsync(command);

        var workOrder = await results.ReadSingleOrDefaultAsync<WorkOrderResponse>();

        if (workOrder is null)
        {
            return null;
        }

        var outputs = (await results.ReadAsync<WorkOrderOutputResponse>()).ToList();

        return workOrder with { Outputs = outputs };
    }

    public async Task<int?> CreateAsync(WorkOrder workOrder, IReadOnlyList<InventoryItem> outputs, CancellationToken cancellationToken)
    {
        const string consumeInputSql = $"""
            UPDATE InventoryItems
            SET {nameof(InventoryItem.Status)} = @ConsumedStatus
            WHERE {nameof(InventoryItem.Id)} = @Id
                AND {nameof(InventoryItem.Status)} = @AvailableStatus
            """;

        const string nextNumberSql = """
            SELECT CONCAT('WO-', 1001 + COUNT(*))
            FROM WorkOrders WITH (UPDLOCK, HOLDLOCK)
            """;

        const string insertWorkOrderSql = $"""
            INSERT INTO WorkOrders (
                {nameof(WorkOrder.WorkOrderNumber)},
                {nameof(WorkOrder.InputItemId)},
                {nameof(WorkOrder.CreatedAtUtc)},
                {nameof(WorkOrder.CreatedByUserId)}
            )
            OUTPUT INSERTED.{nameof(WorkOrder.Id)}
            VALUES (
                @{nameof(WorkOrder.WorkOrderNumber)},
                @{nameof(WorkOrder.InputItemId)},
                @{nameof(WorkOrder.CreatedAtUtc)},
                @{nameof(WorkOrder.CreatedByUserId)}
            )
            """;

        const string insertOutputSql = $"""
            INSERT INTO InventoryItems (
                {nameof(InventoryItem.TagNumber)},
                {nameof(InventoryItem.ProductId)},
                {nameof(InventoryItem.LocationId)},
                {nameof(InventoryItem.Weight)},
                {nameof(InventoryItem.TotalCost)},
                {nameof(InventoryItem.Status)},
                {nameof(InventoryItem.CreatedAtUtc)},
                {nameof(InventoryItem.SourceWorkOrderId)}
            )
            VALUES (
                @{nameof(InventoryItem.TagNumber)},
                @{nameof(InventoryItem.ProductId)},
                @{nameof(InventoryItem.LocationId)},
                @{nameof(InventoryItem.Weight)},
                @{nameof(InventoryItem.TotalCost)},
                @{nameof(InventoryItem.Status)},
                @{nameof(InventoryItem.CreatedAtUtc)},
                @{nameof(InventoryItem.SourceWorkOrderId)}
            )
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        var consumeCommand = new CommandDefinition(
            consumeInputSql,
            new
            {
                Id = workOrder.InputItemId,
                ConsumedStatus = InventoryStatus.Consumed,
                AvailableStatus = InventoryStatus.Available,
            },
            transaction,
            cancellationToken: cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(consumeCommand);

        if (rowsAffected == 0)
        {
            return null;
        }

        var numberCommand = new CommandDefinition(nextNumberSql, transaction: transaction, cancellationToken: cancellationToken);
        workOrder.WorkOrderNumber = await connection.QuerySingleAsync<string>(numberCommand);

        var insertWorkOrderCommand = new CommandDefinition(insertWorkOrderSql, workOrder, transaction, cancellationToken: cancellationToken);
        var id = await connection.QuerySingleAsync<int>(insertWorkOrderCommand);

        foreach (var output in outputs)
        {
            output.SourceWorkOrderId = id;
        }

        var insertOutputCommand = new CommandDefinition(insertOutputSql, outputs, transaction, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(insertOutputCommand);

        await transaction.CommitAsync(cancellationToken);

        return id;
    }
}