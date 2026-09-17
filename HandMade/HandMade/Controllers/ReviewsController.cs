using HandMade.Application.Features.Reviews.Orchestrators.SubmitReviewAction;
using HandMade.Application.Features.Reviews.Queries.ValidateReviewEligibility;
using HandMade.Domain.DomainEnums;
using HandMade.Helpers;
using HandMade.ViewModels.Review;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HandMade.Controllers
{
    /// <summary>
    /// Writing reviews. Reading them is public and lives on the storefront.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = nameof(AssignedRole.Client))]
    public class ReviewsController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> SubmitReview(CreateReviewRequestVM request, CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new SubmitReviewOrchestrator(
                    userId,
                    request.TargetType,
                    request.TargetId,
                    request.Rating,
                    request.Title,
                    request.Content),
                cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to submit your review", HttpContext.Request.Path);

            return Created(string.Empty, new { reviewId = response.Data });
        }

        /// <summary>
        /// Whether the caller may review this target — lets the UI show or hide the
        /// review form instead of letting the client discover the rule by failing.
        /// </summary>
        [HttpGet("eligibility")]
        public async Task<ActionResult> GetEligibility(
            [FromQuery] ReviewTargetType targetType,
            [FromQuery] Guid targetId,
            CancellationToken cancellationToken)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var response = await _mediator.Send(
                new ValidateReviewEligibilityQuery(userId, targetType, targetId), cancellationToken);

            if (!response.IsSuccess)
                return response.ErrorCode.ToProblem("Faild to check your review eligibility", HttpContext.Request.Path);

            return Ok(new { canReview = response.Data });
        }
    }
}
