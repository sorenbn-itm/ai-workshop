using MediatR;

namespace CleanCQRSPOC.Domain.Events;

public class ProductCreatedEvent(int productId, string productName) : INotification
{
    public int ProductId { get; } = productId;
    public string ProductName { get; } = productName;
}
