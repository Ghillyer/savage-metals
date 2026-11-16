using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.PurchaseOrders;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrdersController(IPurchaseOrderService purchaseOrderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<PurchaseOrderSummaryResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PurchaseOrderSummaryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await purchaseOrderService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PurchaseOrderResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await purchaseOrderService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PurchaseOrderResponse>> Create(PurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var response = await purchaseOrderService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PurchaseOrderResponse>> Update(int id, PurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        return Ok(await purchaseOrderService.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:int}/receive")]
    [ProducesResponseType<PurchaseOrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PurchaseOrderResponse>> Receive(int id, ReceivePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        return Ok(await purchaseOrderService.ReceiveAsync(id, request, cancellationToken));
    }
}