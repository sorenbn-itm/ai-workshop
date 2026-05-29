using CleanCQRSPOC.Application.Queries;
using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        string? search,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortField sort,
        SortDirection sortDir,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> UpdateAsync(int id, string name, decimal price, CancellationToken cancellationToken = default);
}
