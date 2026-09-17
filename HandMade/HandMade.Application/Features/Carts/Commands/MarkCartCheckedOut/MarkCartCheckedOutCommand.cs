using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.MarkCartCheckedOut
{
    public record MarkCartCheckedOutCommand(Guid CartId) : IRequest<RequestResult<bool>>;

    public class MarkCartCheckedOutCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<MarkCartCheckedOutCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            MarkCartCheckedOutCommand request,
            CancellationToken cancellationToken)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();

            var cart = await _executor.FirstOrDefaultAsync(
                cartRepo.GetByIdWithTracking(request.CartId),
                cancellationToken);

            if (cart is null)
                return RequestResult<bool>.Failed(ErrorCode.CartNotFound);

            cart.CheckedOutAt = DateTime.UtcNow;

            cartRepo.Update(cart);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
