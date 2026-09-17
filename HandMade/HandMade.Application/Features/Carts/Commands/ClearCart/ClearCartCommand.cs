using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Carts.Commands.ClearCart
{
    public record ClearCartCommand(Guid UserId) : IRequest<RequestResult<bool>>;

    public class ClearCartCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<ClearCartCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ClearCartCommand request,
            CancellationToken cancellationToken)
        {
            var itemRepo = _unitOfWork.GetRepository<CartItem>();

            var liveItems = await _executor.ToListAsync(
                itemRepo.GetRangeWithTracking(ci =>
                    ci.UserId == request.UserId && !ci.IsConvertedToOrder),
                cancellationToken);

            if (liveItems.Count == 0)
                return RequestResult<bool>.Success(true);

            foreach (var item in liveItems)
                itemRepo.SoftDelete(item);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
