using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Reports;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("inventory")]
    [ProducesResponseType<InventoryReportResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<InventoryReportResponse>> GetInventory(CancellationToken cancellationToken)
    {
        return Ok(await reportService.GetInventoryReportAsync(cancellationToken));
    }
}