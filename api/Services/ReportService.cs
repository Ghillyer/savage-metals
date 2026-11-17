using SavageMetals.Api.Core.DTOs.Reports;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Core.Repositories;

namespace SavageMetals.Api.Services;

public class ReportService(IReportRepository reportRepository) : IReportService
{
    public async Task<InventoryReportResponse> GetInventoryReportAsync(CancellationToken cancellationToken)
    {
        return await reportRepository.GetInventoryReportAsync(cancellationToken);
    }
}