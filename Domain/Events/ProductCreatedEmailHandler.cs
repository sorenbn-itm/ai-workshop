using CleanCQRSPOC.Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanCQRSPOC.Domain.Events;

public class ProductCreatedEmailHandler(IEmailService emailService) : INotificationHandler<ProductCreatedEvent>
{
    private readonly IEmailService _emailService = emailService;

    public Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        var subject = $"New Product Created: {notification.ProductName}";
        var body = $"Product ID: {notification.ProductId}, Name: {notification.ProductName}";
        var recipient = "info@company.com";
        
        _emailService.SendEmail(recipient, subject, body);
        return Task.CompletedTask;
    }
}
