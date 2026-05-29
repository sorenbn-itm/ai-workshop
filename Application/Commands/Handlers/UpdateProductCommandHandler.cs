
using MediatR;
using CleanCQRSPOC.Presentation.Models;
using CleanCQRSPOC.Infrastructure.Persistence;

namespace CleanCQRSPOC.Application.Commands.Handlers
{

    public class UpdateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, ProductDto?>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.UpdateAsync(request.Id, request.Name, request.Price, cancellationToken);

            if (product == null) return null;
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }
    }
}