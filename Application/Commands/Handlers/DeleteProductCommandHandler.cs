
using MediatR;
using CleanCQRSPOC.Infrastructure.Persistence;

namespace CleanCQRSPOC.Application.Commands.Handlers
{

    public class DeleteProductCommandHandler(IProductRepository productRepository) : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            return await _productRepository.RemoveAsync(request.Id, cancellationToken);
        }
    }
}