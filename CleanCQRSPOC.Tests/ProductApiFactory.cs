using CleanCQRSPOC.Domain.Entities;
using CleanCQRSPOC.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanCQRSPOC.Tests;

/// <summary>
/// Spins up the API in-process pointed at a unique temporary SQLite database file,
/// so each factory instance is fully isolated and uses the same provider as production.
/// </summary>
public class ProductApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"products-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}"
            });
        });
    }

    /// <summary>Resets the Products table to exactly the supplied set.</summary>
    public void SeedProducts(params Product[] products)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Products.RemoveRange(db.Products);
        db.Products.AddRange(products);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch (IOException) { /* best-effort cleanup */ }
        }
    }
}
