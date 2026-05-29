using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Tests;

public class ProductsEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private sealed record ProductDtoModel(int Id, string Name, decimal Price);

    private sealed record PagedResultModel(
        List<ProductDtoModel> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages);

    private static Product[] SampleProducts() =>
    [
        new Product { Name = "Apple", Price = 0.99m },
        new Product { Name = "Banana", Price = 0.59m },
        new Product { Name = "Cherry", Price = 3.20m },
        // Two-digit price: lexically "10.00" sorts before "3.20"/"2.50", so this guards
        // against decimal-as-TEXT lexical ordering in SQLite.
        new Product { Name = "Date", Price = 10.00m },
        new Product { Name = "Pineapple", Price = 2.50m },
    ];

    private static async Task<PagedResultModel> GetPageAsync(HttpClient client, string query)
    {
        var response = await client.GetAsync($"/api/product{query}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PagedResultModel>(JsonOptions);
        Assert.NotNull(body);
        return body!;
    }

    [Fact]
    public async Task Get_WithDefaults_ReturnsEnvelopeWithAllItemsOnPageOne()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "");

        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
        Assert.Equal(5, result.Items.Count);
    }

    [Fact]
    public async Task Get_WithPaging_ReturnsRequestedSliceAndTotals()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var page1 = await GetPageAsync(client, "?page=1&pageSize=2&sort=name&sortDir=asc");
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(5, page1.TotalCount);
        Assert.Equal(3, page1.TotalPages);
        Assert.Equal("Apple", page1.Items[0].Name);
        Assert.Equal("Banana", page1.Items[1].Name);

        var page3 = await GetPageAsync(client, "?page=3&pageSize=2&sort=name&sortDir=asc");
        Assert.Single(page3.Items);
        Assert.Equal("Pineapple", page3.Items[0].Name);
    }

    [Fact]
    public async Task Get_PageBeyondLast_ReturnsEmptyItemsButCorrectTotals()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "?page=99&pageSize=10");

        Assert.Empty(result.Items);
        Assert.Equal(5, result.TotalCount);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task Get_WithSearch_FiltersByNameCaseInsensitively()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "?search=apple");

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, p => Assert.Contains("apple", p.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Get_WithPriceRange_FiltersInclusive()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "?minPrice=0.99&maxPrice=3.20");

        Assert.Equal(3, result.TotalCount);
        Assert.All(result.Items, p => Assert.InRange(p.Price, 0.99m, 3.20m));
    }

    [Fact]
    public async Task Get_SortByPriceDesc_OrdersCorrectly()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "?sort=price&sortDir=desc");

        var prices = result.Items.Select(p => p.Price).ToList();
        Assert.Equal(prices.OrderByDescending(p => p).ToList(), prices);
    }

    [Fact]
    public async Task Get_SortByNameAscIsDefaultDirection()
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var result = await GetPageAsync(client, "?sort=name");

        var names = result.Items.Select(p => p.Name).ToList();
        Assert.Equal(names.OrderBy(n => n).ToList(), names);
    }

    [Theory]
    [InlineData("?page=0")]
    [InlineData("?pageSize=101")]
    [InlineData("?pageSize=0")]
    [InlineData("?sort=bogus")]
    [InlineData("?sortDir=bogus")]
    [InlineData("?minPrice=5&maxPrice=1")]
    [InlineData("?minPrice=-1")]
    public async Task Get_WithInvalidParams_Returns400WithErrorArray(string query)
    {
        using var factory = new ProductApiFactory();
        factory.SeedProducts(SampleProducts());
        var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/product{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = await response.Content.ReadFromJsonAsync<List<string>>(JsonOptions);
        Assert.NotNull(errors);
        Assert.NotEmpty(errors!);
    }
}
