using CleanCQRSPOC.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CleanCQRSPOC.Presentation.Models;
using CleanCQRSPOC.Application.Commands;

namespace CleanCQRSPOC.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<PagedResult<ProductDto>>> Get([FromQuery] ProductQueryParameters query)
    {
        var result = await _mediator.Send(new GetProductsQuery
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Search = query.Search,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            Sort = ParseSortField(query.Sort),
            SortDir = ParseSortDirection(query.SortDir)
        });
        return Ok(result);
    }

    private static ProductSortField ParseSortField(string? sort) =>
        string.Equals(sort, "price", StringComparison.OrdinalIgnoreCase)
            ? ProductSortField.Price
            : ProductSortField.Name;

    private static SortDirection ParseSortDirection(string? sortDir) =>
        string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase)
            ? SortDirection.Desc
            : SortDirection.Asc;

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> Post([FromBody] CreateProductCommand command)
    {
        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ProductDto>> Put(int id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest();
        var product = await _mediator.Send(command);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        if (!result) return NotFound();
        return NoContent();
    }

}