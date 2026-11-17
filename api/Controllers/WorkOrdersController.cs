using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.WorkOrders;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/work-orders")]
public class WorkOrdersController(IWorkOrderService workOrderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<WorkOrderSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WorkOrderSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await workOrderService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkOrderResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await workOrderService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<WorkOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<WorkOrderResponse>> Create(WorkOrderRequest request, CancellationToken cancellationToken)
    {
        var response = await workOrderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}