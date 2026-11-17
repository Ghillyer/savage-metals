using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Inventory;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/inventory")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<InventoryItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<InventoryItemResponse>>> GetAll([FromQuery] InventoryStatus? status, CancellationToken cancellationToken)
    {
        return Ok(await inventoryService.GetAllAsync(status, cancellationToken));
    }
}