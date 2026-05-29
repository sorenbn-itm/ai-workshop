using CleanCQRSPOC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using CleanCQRSPOC.Presentation.Filters;
using CleanCQRSPOC.Presentation.Models;
using CleanCQRSPOC.Presentation.Validators;
using CleanCQRSPOC.Domain.Events;

namespace CleanCQRSPOC;

public static partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
            options.Filters.Add<LoggingFilter>();
        });

        builder.Services.AddScoped<IValidator<ProductDto>, ProductDtoValidator>();
        builder.Services.AddScoped<ValidationFilter>();
        builder.Services.AddScoped<LoggingFilter>();
        builder.Services.AddScoped<CleanCQRSPOC.Application.Services.IEmailService, CleanCQRSPOC.Application.Services.EmailService>();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<AppDbContext>();
            cfg.RegisterServicesFromAssemblyContaining<ProductCreatedEmailHandler>();
        });


        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=products.db"));

        builder.Services.AddScoped<IProductRepository, ProductRepository>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbInitializer.Initialize(db);
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}