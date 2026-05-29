using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Presentation.Models;
using MediatR;

namespace CleanCQRSPOC.Application.Queries.Handlers;

public class GetOrdersByCustomerQueryHandler(IOrderRepository orderRepository, ICustomerRepository customerRepository)
    : IRequestHandler<GetOrdersByCustomerQuery, List<OrderDto>>
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICustomerRepository _customerRepository = customerRepository;

    public async Task<List<OrderDto>> Handle(GetOrdersByCustomerQuery request, CancellationToken cancellationToken)
    {
        var all = await _orderRepository.GetAllAsync(cancellationToken);
        var filtered = all.Where(o =>
            o.CustomerId == request.CustomerId &&
            (string.IsNullOrEmpty(request.Status) || o.Status == request.Status)
        );

        var enriched = new List<OrderDto>();
        foreach (var order in filtered)
        {
            var customer = await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);
            enriched.Add(new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                Status = order.Status,
                CustomerName = customer?.Name ?? string.Empty
            });
        }
        return enriched;
    }
}
