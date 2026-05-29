using MediatR;
using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Queries.Handlers;

public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(
            request.Search,
            request.MinPrice,
            request.MaxPrice,
            request.Sort,
            request.SortDir,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = [.. products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })],
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
