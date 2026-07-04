using HandMade.Application.Features.Reviews.Queries.GetReviewSummary.DTO;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Reviews.Queries.GetReviewSummary
{
    public record GetReviewSummaryQuery(ReviewTargetType TargetType,Guid TargetId,ReviewStatus Status) : IRequest<RequestResult<ReviewSummaryDTO>>;

    internal class GetReviewSummaryQueryHandler(IUnitOfWork unitOfWork,IQueryableExecutor executor) :
        IRequestHandler<GetReviewSummaryQuery, RequestResult<ReviewSummaryDTO>>
    {
        public async Task<RequestResult<ReviewSummaryDTO>> Handle(GetReviewSummaryQuery request,CancellationToken cancellationToken)
        {
            var filtered = unitOfWork.GetRepository<Review>()
                .GetAll()
                .Where(r => r.TargetType == request.TargetType
                         && r.TargetId == request.TargetId
                         && r.Status == request.Status);

            var aggregateQuery = filtered
                .GroupBy(r => 1)
                .Select(g => new ReviewSummaryDTO
                {
                    ReviewCount = g.Count(),
                    AverageRating = g.Average(r => (double?)r.Rating)
                });

            var rows = await executor.ToListAsync(aggregateQuery, cancellationToken);

            var summary = rows.FirstOrDefault()
                ?? new ReviewSummaryDTO { ReviewCount = 0, AverageRating = null };

            return RequestResult<ReviewSummaryDTO>.Success(summary);
        }
    }

}
