using MediatR;
using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Queries.Handlers;

public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        return [.. products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })];
    }
}