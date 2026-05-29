using CleanCQRSPOC.Application.Queries;
using CleanCQRSPOC.Presentation.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanCQRSPOC.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("by-customer/{customerId}")]
    [ProducesResponseType(typeof(List<OrderDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<List<OrderDto>>> GetByCustomer(int customerId, [FromQuery] string? status)
    {
        var orders = await _mediator.Send(new GetOrdersByCustomerQuery(customerId, status));
        return Ok(orders);
    }
}
