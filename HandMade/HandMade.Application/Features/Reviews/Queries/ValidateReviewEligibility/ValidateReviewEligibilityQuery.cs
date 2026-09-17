using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Reviews.Queries.ValidateReviewEligibility
{
    /// <summary>
    /// Only buyers who actually received the goods may review them. Derived from the
    /// orders table rather than a flag on Review, so a "verified purchase" cannot be
    /// faked: there must be a Delivered order from this user against this target.
    ///
    /// Buyer reviews are the artist reviewing a client, so they are not eligible here.
    /// </summary>
    public record ValidateReviewEligibilityQuery(
        Guid UserId,
        ReviewTargetType TargetType,
        Guid TargetId) : IRequest<RequestResult<bool>>;

    public class ValidateReviewEligibilityQueryHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<ValidateReviewEligibilityQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateReviewEligibilityQuery request,
            CancellationToken cancellationToken)
        {
            var orderRepo = _unitOfWork.GetRepository<Order>();

            var eligible = request.TargetType switch
            {
                ReviewTargetType.Product => await orderRepo.AnyAsync(
                    o => o.UserId == request.UserId
                         && o.ProductId == request.TargetId
                         && o.Status == OrderStatus.Delivered,
                    cancellationToken),

                ReviewTargetType.Shop => await orderRepo.AnyAsync(
                    o => o.UserId == request.UserId
                         && o.ShopId == request.TargetId
                         && o.Status == OrderStatus.Delivered,
                    cancellationToken),

                _ => false
            };

            return RequestResult<bool>.Success(eligible);
        }
    }
}
