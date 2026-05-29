using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Presentation.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CleanCQRSPOC.Tests;

public class ProductEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public async Task GetProducts_ReturnsDefaultPagedEnvelope()
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Product");

        var page = await ReadPageAsync(response);
        Assert.Equal(1, page.Page);
        Assert.Equal(20, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(1, page.TotalPages);
        Assert.Collection(page.Items,
            product => Assert.Equal("Apple", product.Name),
            product => Assert.Equal("Banana", product.Name));
    }

    [Fact]
    public async Task GetProducts_FiltersBySearchAndPriceRange()
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Product?search=app&minPrice=0.5&maxPrice=1");

        var page = await ReadPageAsync(response);
        var product = Assert.Single(page.Items);
        Assert.Equal("Apple", product.Name);
        Assert.Equal(1, page.TotalCount);
    }

    [Fact]
    public async Task GetProducts_SortsByNameDescending()
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Product?sort=name&sortDir=desc");

        var page = await ReadPageAsync(response);
        Assert.Collection(page.Items,
            product => Assert.Equal("Banana", product.Name),
            product => Assert.Equal("Apple", product.Name));
    }

    [Fact]
    public async Task GetProducts_SortsByPriceDescending()
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Product?sort=price&sortDir=desc");

        var page = await ReadPageAsync(response);
        Assert.Collection(page.Items,
            product => Assert.Equal("Apple", product.Name),
            product => Assert.Equal("Banana", product.Name));
    }

    [Fact]
    public async Task GetProducts_PaginatesResults()
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Product?page=2&pageSize=1&sort=name");

        var page = await ReadPageAsync(response);
        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(2, page.TotalPages);
        var product = Assert.Single(page.Items);
        Assert.Equal("Banana", product.Name);
    }

    [Theory]
    [InlineData("page=0", "Page must be greater than or equal to 1.")]
    [InlineData("pageSize=101", "PageSize must be between 1 and 100.")]
    [InlineData("sort=created", "Sort must be one of: name, price.")]
    [InlineData("sortDir=sideways", "SortDir must be one of: asc, desc.")]
    [InlineData("minPrice=2&maxPrice=1", "MinPrice must be less than or equal to MaxPrice.")]
    public async Task GetProducts_RejectsInvalidQueryValues(string queryString, string expectedMessage)
    {
        using var factory = new ProductApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/Product?{queryString}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(expectedMessage, body);
    }

    private static async Task<PagedResponse<ProductDto>> ReadPageAsync(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>(JsonOptions);
        Assert.NotNull(page);
        return page;
    }

    private sealed class ProductApiFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection? _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();

                _connection = new SqliteConnection("Data Source=:memory:");
                _connection.Open();

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(_connection));
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
