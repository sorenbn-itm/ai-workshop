using MediatR;

namespace CleanCQRSPOC.Application.Commands
{
    public class DeleteProductCommand(int id) : IRequest<bool>
    {
        public int Id { get; set; } = id;
    }
}