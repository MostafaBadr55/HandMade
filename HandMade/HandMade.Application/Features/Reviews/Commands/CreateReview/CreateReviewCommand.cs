using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Reviews.Commands.CreateReview
{
    public record CreateReviewCommand(
        Guid ReviewerUserId,
        ReviewTargetType TargetType,
        Guid TargetId,
        int Rating,
        string Title,
        string Content) : IRequest<RequestResult<Guid>>;

    public class CreateReviewCommandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<CreateReviewCommand, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            CreateReviewCommand request,
            CancellationToken cancellationToken)
        {
            if (request.Rating is < 1 or > 5)
                return RequestResult<Guid>.Failed(ErrorCode.InvalidRating);

            var reviewRepo = _unitOfWork.GetRepository<Review>();

            // Mirrors the unique index on (ReviewerUserId, TargetType, TargetId) —
            // caught here so the caller gets a 409 instead of a DB exception.
            var alreadyReviewed = await reviewRepo.AnyAsync(
                r => r.ReviewerUserId == request.ReviewerUserId
                     && r.TargetType == request.TargetType
                     && r.TargetId == request.TargetId,
                cancellationToken);

            if (alreadyReviewed)
                return RequestResult<Guid>.Failed(ErrorCode.AlreadyReviewed);

            var review = new Review
            {
                ReviewerUserId = request.ReviewerUserId,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Rating = request.Rating,
                Title = request.Title,
                Content = request.Content,
                Status = ReviewStatus.Approved
            };

            reviewRepo.Add(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(review.Id);
        }
    }
}
