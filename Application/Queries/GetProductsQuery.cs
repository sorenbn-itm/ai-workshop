using MediatR;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Queries;

public class GetProductsQuery : IRequest<List<ProductDto>>
{
}