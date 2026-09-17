using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.MarkCartItemConverted
{
    public record MarkCartItemConvertedCommand(Guid CartItemId, Guid OrderId) : IRequest<RequestResult<bool>>;

    public class MarkCartItemConvertedCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<MarkCartItemConvertedCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            MarkCartItemConvertedCommand request,
            CancellationToken cancellationToken)
        {
            var itemRepo = _unitOfWork.GetRepository<CartItem>();

            var item = await _executor.FirstOrDefaultAsync(
                itemRepo.GetByIdWithTracking(request.CartItemId),
                cancellationToken);

            if (item is null)
                return RequestResult<bool>.Failed(ErrorCode.CartItemNotFound);

            if (item.IsConvertedToOrder)
                return RequestResult<bool>.Failed(ErrorCode.CartItemAlreadyConverted);

            item.OrderId = request.OrderId;
            item.IsConvertedToOrder = true;

            itemRepo.Update(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
