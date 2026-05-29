using MediatR;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Commands
{
    public class UpdateProductCommand : IRequest<ProductDto?>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}