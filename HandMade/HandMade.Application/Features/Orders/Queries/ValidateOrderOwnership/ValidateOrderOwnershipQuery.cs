using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.ValidateOrderOwnership
{
    /// <summary>
    /// True when the order exists and belongs to this buyer. Reused by every
    /// client-side order transition.
    /// </summary>
    public record ValidateOrderOwnershipQuery(Guid UserId, Guid OrderId) : IRequest<RequestResult<bool>>;

    public class ValidateOrderOwnershipQueryHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<ValidateOrderOwnershipQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateOrderOwnershipQuery request,
            CancellationToken cancellationToken)
        {
            var owned = await _unitOfWork
                .GetRepository<Order>()
                .AnyAsync(o => o.Id == request.OrderId && o.UserId == request.UserId, cancellationToken);

            return RequestResult<bool>.Success(owned);
        }
    }
}
