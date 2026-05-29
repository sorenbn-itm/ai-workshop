using CleanCQRSPOC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
