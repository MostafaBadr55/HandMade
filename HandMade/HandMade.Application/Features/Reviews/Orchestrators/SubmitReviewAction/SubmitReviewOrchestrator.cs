using HandMade.Application.Features.Reviews.Commands.CreateReview;
using HandMade.Application.Features.Reviews.Queries.ValidateReviewEligibility;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using MediatR;

namespace HandMade.Application.Features.Reviews.Orchestrators.SubmitReviewAction
{
    /// <summary>
    /// Check the buyer earned the right to review, then record it. Eligibility is a
    /// separate step because it is worth reusing on the read side too (to show or
    /// hide a "write a review" affordance).
    /// </summary>
    public record SubmitReviewOrchestrator(
        Guid ReviewerUserId,
        ReviewTargetType TargetType,
        Guid TargetId,
        int Rating,
        string Title,
        string Content) : IRequest<RequestResult<Guid>>;

    public class SubmitReviewOrchestratorHandler(IMediator _mediator)
        : IRequestHandler<SubmitReviewOrchestrator, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            SubmitReviewOrchestrator request,
            CancellationToken cancellationToken)
        {
            var eligible = await _mediator.Send(
                new ValidateReviewEligibilityQuery(request.ReviewerUserId, request.TargetType, request.TargetId),
                cancellationToken);

            if (!eligible.IsSuccess)
                return RequestResult<Guid>.Failed(eligible.ErrorCode);

            if (!eligible.Data)
                return RequestResult<Guid>.Failed(ErrorCode.NotEligibleToReview);

            return await _mediator.Send(
                new CreateReviewCommand(
                    request.ReviewerUserId,
                    request.TargetType,
                    request.TargetId,
                    request.Rating,
                    request.Title,
                    request.Content),
                cancellationToken);
        }
    }
}
