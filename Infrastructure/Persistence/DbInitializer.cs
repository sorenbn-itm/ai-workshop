using CleanCQRSPOC.Domain.Entities;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Products.AddRange(
            new Product { Name = "Apple", Price = 0.99m },
            new Product { Name = "Banana", Price = 0.59m }
        );

        var alice = new Customer { Name = "Alice" };
        var bob = new Customer { Name = "Bob" };
        context.Customers.AddRange(alice, bob);
        context.SaveChanges();

        context.Orders.AddRange(
            new Order { CustomerId = alice.Id, Status = "pending" },
            new Order { CustomerId = alice.Id, Status = "shipped" },
            new Order { CustomerId = bob.Id, Status = "pending" }
        );
        context.SaveChanges();
    }
}