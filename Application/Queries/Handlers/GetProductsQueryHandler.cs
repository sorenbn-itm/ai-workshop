using MediatR;
using CleanCQRSPOC.Infrastructure.Persistence;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Queries.Handlers;

public class GetProductsQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsQuery, PagedResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<PagedResponse<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.MinPrice,
            request.MaxPrice,
            request.Sort,
            request.SortDir,
            cancellationToken);

        return new PagedResponse<ProductDto>
        {
            Items = [.. products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            })],
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}