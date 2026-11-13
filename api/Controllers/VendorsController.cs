using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Vendors;
using SavageMetals.Api.Core.Enums;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/vendors")]
public class VendorsController(IVendorService vendorService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<VendorResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<VendorResponse>>> GetAll([FromQuery] bool includeInactive, [FromQuery] VendorType? type, CancellationToken cancellationToken)
    {
        var vendors = await vendorService.GetAllAsync(includeInactive, type, cancellationToken);
        return Ok(vendors);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<VendorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var vendor = await vendorService.GetByIdAsync(id, cancellationToken);
        return Ok(vendor);
    }

    [HttpPost]
    [ProducesResponseType<VendorResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VendorResponse>> Create(VendorRequest request, CancellationToken cancellationToken)
    {
        var vendor = await vendorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = vendor.Id }, vendor);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<VendorResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<VendorResponse>> Update(int id, VendorRequest request, CancellationToken cancellationToken)
    {
        var vendor = await vendorService.UpdateAsync(id, request, cancellationToken);
        return Ok(vendor);
    }
}