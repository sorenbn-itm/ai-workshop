using MediatR;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}