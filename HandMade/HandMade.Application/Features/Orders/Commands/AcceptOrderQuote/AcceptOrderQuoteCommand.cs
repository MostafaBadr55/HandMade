using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.AcceptOrderQuote
{
    /// <summary>
    /// BuyerPending -> InProgress. Only ever called after the payment has been taken,
    /// so acceptance and escrow stay consistent.
    /// </summary>
    public record AcceptOrderQuoteCommand(Guid UserId, Guid OrderId) : IRequest<RequestResult<bool>>;

    public class AcceptOrderQuoteCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<AcceptOrderQuoteCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            AcceptOrderQuoteCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var order = await _executor.FirstOrDefaultAsync(
                orderRepo.GetByIdWithTracking(request.OrderId),
                cancellationToken);

            if (order is null)
                return RequestResult<bool>.Failed(ErrorCode.OrderNotFound);

            if (order.UserId != request.UserId)
                return RequestResult<bool>.Failed(ErrorCode.OrderAccessDenied);

            if (order.Status != OrderStatus.BuyerPending)
                return RequestResult<bool>.Failed(ErrorCode.InvalidOrderStatusTransition);

            order.Status = OrderStatus.InProgress;
            order.ConfirmedAt = DateTime.UtcNow;

            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
