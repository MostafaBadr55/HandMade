using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.GetOrCreateCart
{
    /// <summary>
    /// Returns the caller's cart id, creating the cart on first use. A user has
    /// exactly one cart row for life (Cart.UserId is a unique FK), so checking out
    /// does not retire it — adding an item afterwards reopens the same cart by
    /// clearing CheckedOutAt.
    /// </summary>
    public record GetOrCreateCartCommand(Guid UserId) : IRequest<RequestResult<Guid>>;

    public class GetOrCreateCartCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<GetOrCreateCartCommand, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            GetOrCreateCartCommand request,
            CancellationToken cancellationToken)
        {
            var cartRepo = _unitOfWork.GetRepository<Cart>();

            var cart = await _executor.FirstOrDefaultAsync(
                cartRepo.GetRangeWithTracking(c => c.UserId == request.UserId),
                cancellationToken);

            if (cart is null)
            {
                cart = new Cart { UserId = request.UserId };
                cartRepo.Add(cart);
            }
            else if (cart.CheckedOutAt is not null)
            {
                // Reopen a cart that was previously checked out.
                cart.CheckedOutAt = null;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(cart.Id);
        }
    }
}
