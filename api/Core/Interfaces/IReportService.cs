using SavageMetals.Api.Core.DTOs.Reports;

namespace SavageMetals.Api.Core.Interfaces;

public interface IReportService
{
    Task<InventoryReportResponse> GetInventoryReportAsync(CancellationToken cancellationToken = default);
}