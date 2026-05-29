using CleanCQRSPOC.Application.Queries;
using CleanCQRSPOC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(
        string? search,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortField sort,
        SortDirection sortDir,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(term));
        }

        // SQLite stores decimal as TEXT, so price comparisons/ordering must run on a numeric
        // cast (translated to CAST(... AS REAL)) to compare by value rather than lexically.
        if (minPrice.HasValue)
        {
            var min = (double)minPrice.Value;
            query = query.Where(p => (double)p.Price >= min);
        }

        if (maxPrice.HasValue)
        {
            var max = (double)maxPrice.Value;
            query = query.Where(p => (double)p.Price <= max);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = (sort, sortDir) switch
        {
            (ProductSortField.Price, SortDirection.Desc) => query.OrderByDescending(p => (double)p.Price),
            (ProductSortField.Price, _) => query.OrderBy(p => (double)p.Price),
            (ProductSortField.Name, SortDirection.Desc) => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Name),
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Product?> UpdateAsync(int id, string name, decimal price, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product == null) return null;
        product.Name = name;
        product.Price = price;
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }
}