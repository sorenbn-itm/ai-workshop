using CleanCQRSPOC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanCQRSPOC.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}