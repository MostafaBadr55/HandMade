using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Queries.GetOrdersDueForAutoRelease
{
    /// <summary>
    /// Orders the artist finished that the client never confirmed, whose grace period
    /// has now expired. Drives the escrow sweep so an unresponsive buyer cannot hold
    /// the artist money indefinitely.
    /// </summary>
    public record GetOrdersDueForAutoReleaseQuery(int BatchSize = 50) : IRequest<RequestResult<List<Guid>>>;

    public class GetOrdersDueForAutoReleaseQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<GetOrdersDueForAutoReleaseQuery, RequestResult<List<Guid>>>
    {
        public async Task<RequestResult<List<Guid>>> Handle(
            GetOrdersDueForAutoReleaseQuery request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            var orderIds = await _executor.ToListAsync(
                _unitOfWork.GetRepository<Order>()
                    .Get(o => o.Status == OrderStatus.CompletedBySeller
                              && o.AutoReleaseAt != null
                              && o.AutoReleaseAt <= now)
                    .OrderBy(o => o.AutoReleaseAt)
                    .Take(request.BatchSize)
                    .Select(o => o.Id),
                cancellationToken);

            return RequestResult<List<Guid>>.Success(orderIds);
        }
    }
}
