using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();
        if (context.Products.Any()) return;
        context.Products.AddRange(
            new Product { Name = "Apple", Price = 0.99m },
            new Product { Name = "Banana", Price = 0.59m }
        );
        context.SaveChanges();
    }
}