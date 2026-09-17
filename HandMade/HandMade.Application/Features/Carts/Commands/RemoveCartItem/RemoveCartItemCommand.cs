using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.RemoveCartItem
{
    public record RemoveCartItemCommand(Guid UserId, Guid CartItemId) : IRequest<RequestResult<bool>>;

    public class RemoveCartItemCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<RemoveCartItemCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            RemoveCartItemCommand request,
            CancellationToken cancellationToken)
        {
            var itemRepo = _unitOfWork.GetRepository<CartItem>();

            var item = await _executor.FirstOrDefaultAsync(
                itemRepo.GetByIdWithTracking(request.CartItemId),
                cancellationToken);

            if (item is null || item.UserId != request.UserId)
                return RequestResult<bool>.Failed(ErrorCode.CartItemNotFound);

            // A converted line is the record of an order that exists — removing it
            // would orphan that history.
            if (item.IsConvertedToOrder)
                return RequestResult<bool>.Failed(ErrorCode.CartItemAlreadyConverted);

            itemRepo.SoftDelete(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
