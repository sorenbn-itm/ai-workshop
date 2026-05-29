using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Order>> GetByCustomerIdAsync(int customerId, string? status = null, CancellationToken cancellationToken = default);
}
