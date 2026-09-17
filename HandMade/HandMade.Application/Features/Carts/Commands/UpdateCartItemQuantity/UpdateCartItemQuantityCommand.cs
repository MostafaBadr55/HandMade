using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.UpdateCartItemQuantity
{
    public record UpdateCartItemQuantityCommand(
        Guid UserId,
        Guid CartItemId,
        int Quantity) : IRequest<RequestResult<bool>>;

    public class UpdateCartItemQuantityCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<UpdateCartItemQuantityCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            UpdateCartItemQuantityCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
                return RequestResult<bool>.Failed(ErrorCode.InvalidQuantity);

            var itemRepo = _unitOfWork.GetRepository<CartItem>();

            var item = await _executor.FirstOrDefaultAsync(
                itemRepo.GetByIdWithTracking(request.CartItemId),
                cancellationToken);

            if (item is null)
                return RequestResult<bool>.Failed(ErrorCode.CartItemNotFound);

            if (item.UserId != request.UserId)
                return RequestResult<bool>.Failed(ErrorCode.CartItemNotFound);

            if (item.IsConvertedToOrder)
                return RequestResult<bool>.Failed(ErrorCode.CartItemAlreadyConverted);

            item.Quantity = request.Quantity;
            item.TotalPrice = item.UnitPrice * request.Quantity;

            itemRepo.Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
