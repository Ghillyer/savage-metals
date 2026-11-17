using Dapper;
using SavageMetals.Api.Core.DTOs.Reports;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.DAL;

public class ReportRepository(IDbConnectionFactory connectionFactory) : IReportRepository
{
    public async Task<InventoryReportResponse> GetInventoryReportAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                p.Name,
                COUNT(*) AS ItemCount,
                SUM(ii.Weight) AS TotalWeight,
                CAST(SUM(ii.TotalCost) AS decimal(18, 2)) AS TotalValue
            FROM InventoryItems ii
            INNER JOIN Products p ON p.Id = ii.ProductId
            WHERE ii.Status = @AvailableStatus
            GROUP BY p.Name
            ORDER BY p.Name;

            SELECT
                l.Name,
                COUNT(*) AS ItemCount,
                SUM(ii.Weight) AS TotalWeight,
                CAST(SUM(ii.TotalCost) AS decimal(18, 2)) AS TotalValue
            FROM InventoryItems ii
            INNER JOIN Locations l ON l.Id = ii.LocationId
            WHERE ii.Status = @AvailableStatus
            GROUP BY l.Name
            ORDER BY l.Name;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { AvailableStatus = InventoryStatus.Available }, cancellationToken: cancellationToken);
        using var results = await connection.QueryMultipleAsync(command);

        var byProduct = (await results.ReadAsync<InventoryReportRowResponse>()).ToList();
        var byLocation = (await results.ReadAsync<InventoryReportRowResponse>()).ToList();

        return new InventoryReportResponse
        {
            ItemCount = byProduct.Sum(row => row.ItemCount),
            TotalWeight = byProduct.Sum(row => row.TotalWeight),
            TotalValue = byProduct.Sum(row => row.TotalValue),
            ByProduct = byProduct,
            ByLocation = byLocation,
        };
    }
}