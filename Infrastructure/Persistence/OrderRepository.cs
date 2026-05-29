using CleanCQRSPOC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public class OrderRepository(AppDbContext context) : IOrderRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders.ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> GetByCustomerIdAsync(int customerId, string? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Orders.Where(o => o.CustomerId == customerId);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);
        return await query.ToListAsync(cancellationToken);
    }
}
