using HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetPublicReviews
{
    public record GetReviewsQuery(ReviewsCriteria criteria, int pageNumber, int pageSize) : IRequest<RequestResult<PagedResult<ReviewItemDTO>>>;

    internal class GetReviewsQueryHandler(IUnitOfWork unitOfWork,IQueryableExecutor executor,IAccountServices accountServices) : IRequestHandler<GetReviewsQuery, RequestResult<PagedResult<ReviewItemDTO>>>
    {
        public async Task<RequestResult<PagedResult<ReviewItemDTO>>> Handle(GetReviewsQuery request,
            CancellationToken cancellationToken)
        {
            var spec = new ReviewsSpecification(request.criteria);
            var query = unitOfWork.GetRepository<Review>()
           .GetAll()
           .ApplySpecification(spec)
           .Select(r => new
           {
               r.ReviewerUserId,
               r.Title,
               r.Content,
               r.Rating
           });

            var pagedRows = 
                await query.ToPagedResultAsync(executor, request.pageNumber, request.pageSize, cancellationToken);

            var reviewerIds = pagedRows.Items.Select(r => r.ReviewerUserId).Distinct();
            var usernames = await accountServices.GetUsernamesByIdsAsync(reviewerIds);

            var items = pagedRows.Items.Select(r => new ReviewItemDTO
            {
                ReviewerName = usernames.TryGetValue(r.ReviewerUserId, out var name) ? name : "Unknown",
                ReviewTitle = r.Title,
                ReviewContent = r.Content,
                Rating = r.Rating
            }).ToList();

            var result = new PagedResult<ReviewItemDTO>
            {
                Items = items,
                TotalCount = pagedRows.TotalCount,
                PageNumber = pagedRows.PageNumber,
                PageSize = pagedRows.PageSize
            };

            return RequestResult<PagedResult<ReviewItemDTO>>.Success(result);
        }
    }
}
