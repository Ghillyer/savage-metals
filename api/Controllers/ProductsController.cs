using Microsoft.AspNetCore.Mvc;
using SavageMetals.Api.Core.DTOs.Products;
using SavageMetals.Api.Core.Interfaces;

namespace SavageMetals.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProductResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync(includeInactive, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Create(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Update(int id, ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.UpdateAsync(id, request, cancellationToken);
        return Ok(product);
    }
}