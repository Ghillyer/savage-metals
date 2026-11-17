using SavageMetals.Api.Core.DTOs.Reports;

namespace SavageMetals.Api.Core.Repositories;

public interface IReportRepository
{
    Task<InventoryReportResponse> GetInventoryReportAsync(CancellationToken cancellationToken = default);
}