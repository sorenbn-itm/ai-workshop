
using MediatR;
using CleanCQRSPOC.Presentation.Models;
using CleanCQRSPOC.Domain.Entities;
using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Domain.Events;

namespace CleanCQRSPOC.Application.Commands.Handlers
{

    public class CreateProductCommandHandler(IProductRepository productRepository, IMediator mediator) : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly IMediator _mediator = mediator;

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price
            };

            await _productRepository.AddAsync(product, cancellationToken);

            await _mediator.Publish(new ProductCreatedEvent(product.Id, product.Name), cancellationToken);

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }
    }
}