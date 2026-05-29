using CleanCQRSPOC.Presentation.Models;
using MediatR;

namespace CleanCQRSPOC.Application.Queries;

public class GetOrdersByCustomerQuery(int customerId, string? status) : IRequest<List<OrderDto>>
{
    public int CustomerId { get; } = customerId;
    public string? Status { get; } = status;
}
