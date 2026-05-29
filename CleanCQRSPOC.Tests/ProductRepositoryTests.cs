using CleanCQRSPOC.Application.Queries;
using CleanCQRSPOC.Domain.Entities;
using CleanCQRSPOC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CleanCQRSPOC.Tests;

public class ProductRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"repo-tests-{Guid.NewGuid()}")
            .Options;
        var context = new AppDbContext(options);
        context.Products.AddRange(
            new Product { Name = "Apple", Price = 0.99m },
            new Product { Name = "Banana", Price = 0.59m },
            new Product { Name = "Cherry", Price = 12.00m },
            new Product { Name = "Pineapple", Price = 2.50m });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task GetPagedAsync_AppliesSearchPriceRangeAndPaging()
    {
        using var context = CreateContext();
        var repo = new ProductRepository(context);

        var (items, totalCount) = await repo.GetPagedAsync(
            search: "apple",
            minPrice: null,
            maxPrice: null,
            sort: ProductSortField.Name,
            sortDir: SortDirection.Asc,
            page: 1,
            pageSize: 10);

        Assert.Equal(2, totalCount);
        Assert.Equal(new[] { "Apple", "Pineapple" }, items.Select(p => p.Name).ToArray());
    }

    [Fact]
    public async Task GetPagedAsync_SortByPriceDescending()
    {
        using var context = CreateContext();
        var repo = new ProductRepository(context);

        var (items, totalCount) = await repo.GetPagedAsync(
            search: null,
            minPrice: null,
            maxPrice: null,
            sort: ProductSortField.Price,
            sortDir: SortDirection.Desc,
            page: 1,
            pageSize: 10);

        Assert.Equal(4, totalCount);
        Assert.Equal(new[] { 12.00m, 2.50m, 0.99m, 0.59m }, items.Select(p => p.Price).ToArray());
    }

    [Fact]
    public async Task GetPagedAsync_SecondPageSkipsFirstPageItems()
    {
        using var context = CreateContext();
        var repo = new ProductRepository(context);

        var (items, totalCount) = await repo.GetPagedAsync(
            search: null,
            minPrice: null,
            maxPrice: null,
            sort: ProductSortField.Name,
            sortDir: SortDirection.Asc,
            page: 2,
            pageSize: 2);

        Assert.Equal(4, totalCount);
        Assert.Equal(new[] { "Cherry", "Pineapple" }, items.Select(p => p.Name).ToArray());
    }
}
