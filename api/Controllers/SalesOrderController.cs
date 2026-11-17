using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.SalesOrders;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/sales-orders")]
public class SalesOrdersController(ISalesOrderService salesOrderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<SalesOrderSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SalesOrderSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await salesOrderService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<SalesOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SalesOrderResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await salesOrderService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<SalesOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SalesOrderResponse>> Create(SalesOrderRequest request, CancellationToken cancellationToken)
    {
        var response = await salesOrderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}