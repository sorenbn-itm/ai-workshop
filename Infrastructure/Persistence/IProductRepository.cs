using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort,
        string sortDir,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> UpdateAsync(int id, string name, decimal price, CancellationToken cancellationToken = default);
}
