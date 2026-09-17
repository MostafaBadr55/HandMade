using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.MarkOrderComplete
{
    /// <summary>
    /// InProgress -> CompletedBySeller. Stamps AutoReleaseAt so an unresponsive
    /// buyer cannot strand the artist's escrowed funds — EscrowAutoReleaseService
    /// sweeps orders past this date and runs the same confirm-delivery flow the
    /// client would trigger manually.
    /// </summary>
    public record MarkOrderCompleteCommand(Guid UserId, Guid OrderId) : IRequest<RequestResult<bool>>;

    public class MarkOrderCompleteCommandHandler(
        IUnitOfWork _unitOfWork,
        IQueryableExecutor _executor,
        IEscrowPolicy _escrowPolicy)
        : IRequestHandler<MarkOrderCompleteCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            MarkOrderCompleteCommand request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var order = await _executor.FirstOrDefaultAsync(
                orderRepo.GetByIdWithTracking(request.OrderId),
                cancellationToken);

            if (order is null)
                return RequestResult<bool>.Failed(ErrorCode.OrderNotFound);

            var isOwner = await _unitOfWork.GetRepository<Shop>()
                .AnyAsync(s => s.Id == order.ShopId && s.OwnerUserId == request.UserId, cancellationToken);

            if (!isOwner)
                return RequestResult<bool>.Failed(ErrorCode.OrderAccessDenied);

            if (order.Status != OrderStatus.InProgress)
                return RequestResult<bool>.Failed(ErrorCode.InvalidOrderStatusTransition);

            order.Status = OrderStatus.CompletedBySeller;
            order.AutoReleaseAt = DateTime.UtcNow.AddDays(_escrowPolicy.AutoReleaseDays);

            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
