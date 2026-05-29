using MediatR;
using CleanCQRSPOC.Presentation.Models;

namespace CleanCQRSPOC.Application.Queries
{
    public class GetProductByIdQuery(int id) : IRequest<ProductDto?>
    {
        public int Id { get; set; } = id;
    }
}