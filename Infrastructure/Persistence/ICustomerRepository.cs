using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
