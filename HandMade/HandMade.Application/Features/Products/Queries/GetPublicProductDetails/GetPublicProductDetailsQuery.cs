using HandMade.Application.Features.ProductImages.Queries.GetProductImages.DTOs;
using HandMade.Application.Features.Products.Queries.GetPublicProductDetails.DTOs;
using HandMade.Application.Features.Reviews.Queries.GetPublicReviews;
using HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO;
using HandMade.Application.Features.Reviews.Queries.GetReviewSummary;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetPublicProductDetails
{
    public record GetPublicProductDetailsQuery(Guid productId) : IRequest<RequestResult<PublicProductDetailsDTO>>;

    internal class GetPublicProductDetailsQueryHanldler(
        IUnitOfWork unitOfWork,IMediator mediator, IAccountServices accountServices, IUrlBuilder urlBuilder) : 
        IRequestHandler<GetPublicProductDetailsQuery, RequestResult<PublicProductDetailsDTO>>
    {
        private const int PreviewReviewCount = 5;

        public async Task<RequestResult<PublicProductDetailsDTO>> Handle(
            GetPublicProductDetailsQuery request,
            CancellationToken cancellationToken)
        {
            //Get the product from the database
            var product = unitOfWork.GetRepository<Product>()
                .GetById(request.productId)
                .Where(p => p.IsPublished
                         && p.ApprovalStatus == ProductApprovalStatus.Approved)
                .Select(p => new PublicProductDetailsDTO
                {
                    ProductId = p.Id,
                    ShopId = p.ShopId,
                    ShopName = p.Shop.Name,
                    ProductName = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    ExpectedDays = p.ExpectedDays,
                    Images = p.ProductImages
                        .OrderBy(pi => pi.SortOrder)
                        .Select(pi => new ProductImageDTO
                        {
                            Id = pi.Id,
                            Url = pi.Url,
                            AltText = pi.AltText,
                            SortOrder = pi.SortOrder,
                            IsPrimary = pi.IsPrimary
                        })
                        .ToList()
                })
                .FirstOrDefault();

            if (product is null)
                return RequestResult<PublicProductDetailsDTO>.Failed(ErrorCode.ProductNotFound);

            //Build image Url
            if (product.Images.Count > 0)
            {
                foreach (var image in product.Images)
                    image.Url = urlBuilder.BuildAbsoluteUrl(image.Url);
            }

            var reviewSummaryResult = await mediator.Send(
                new GetReviewSummaryQuery(ReviewTargetType.Product, request.productId, ReviewStatus.Approved),
                cancellationToken);

            if (!reviewSummaryResult.IsSuccess)
                return RequestResult<PublicProductDetailsDTO>.Failed(reviewSummaryResult.ErrorCode);

            var reviewsCriteria = new ReviewsCriteria
            {
                TargetType = ReviewTargetType.Product,
                TargetId = request.productId,
                Status = ReviewStatus.Approved,
                RatingFilter = null,
                SortBy = ReviewSortBy.RatingDescending
            };

            var reviewsResult = await mediator.Send(
                new GetReviewsQuery(reviewsCriteria, pageNumber: 1, pageSize: PreviewReviewCount),
                cancellationToken);

            if (!reviewsResult.IsSuccess)
                return RequestResult<PublicProductDetailsDTO>.Failed(reviewsResult.ErrorCode);

            product.AverageRating = reviewSummaryResult.Data!.AverageRating;
            product.ReviewCount = reviewSummaryResult.Data.ReviewCount;
            product.Reviews = reviewsResult.Data!.Items
                .Select(r => new PublicProductReviewDTO
                {
                    ReviewerName = r.ReviewerName,
                    ReviewTitle = r.ReviewTitle,
                    ReviewContent = r.ReviewContent,
                    Rating = r.Rating
                })
                .ToList();

            return RequestResult<PublicProductDetailsDTO>.Success(product);
        }
    }
}
