using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Locations;
using SavageMetals.Api.Core.Interfaces;
using SavageMetals.Api.Security;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController(ILocationService locationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<LocationResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LocationResponse>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var locations = await locationService.GetAllAsync(includeInactive, cancellationToken);
        return Ok(locations);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var location = await locationService.GetByIdAsync(id, cancellationToken);
        return Ok(location);
    }

    [HttpPost]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocationResponse>> Create(LocationRequest request, CancellationToken cancellationToken)
    {
        var location = await locationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = location.Id }, location);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Policies.AdminOnly)]
    [ProducesResponseType<LocationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocationResponse>> Update(int id, LocationRequest request, CancellationToken cancellationToken)
    {
        var location = await locationService.UpdateAsync(id, request, cancellationToken);
        return Ok(location);
    }
}