using HandMade.Application.Features.Categories.Queries.GetCategoryManagementDashboard;
using HandMade.Application.Features.HomePage.Queries;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Features.Products.Queries.GetPublicProductDetails;
using HandMade.Application.Features.Reviews.Queries.GetPublicReviews;
using HandMade.Application.Features.Reviews.Queries.GetPublicReviews.DTO;
using HandMade.Application.Features.Reviews.Queries.GetReviewSummary;
using HandMade.Domain.DomainEnums;
using HandMade.Application.Interfaces;
using HandMade.Helpers;
using HandMade.ViewModels.Category;
using HandMade.ViewModels.ProductImage;
using HandMade.ViewModels.Products;
using HandMade.ViewModels.Review;
using HandMade.ViewModels.StoreFront;
using HandMade.ViewModels.SubCategory;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreFrontController(IMediator mediator) : ControllerBase
    {
        [HttpGet("home")]
        public async Task<ActionResult> GetHomePage(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetHomePageQuery(), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get the home page", HttpContext.Request.Path);

            var response = new HomePageResponseVM
            {
                Categories = result.Data!.Categories
                    .Select(c => new CategoryCardResponseVM
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        CategoryImage = c.CategoryImage
                    }).ToList(),

                TopRatedShops = result.Data!.TopRatedShops
                    .Select(s => new ShopCardResponseVM
                    {
                        ShopId = s.ShopId,
                        ShopName = s.ShopName,
                        Description = s.Description,
                        MainImage = s.MainImage,
                        Rating = s.Rating
                    }).ToList(),

                MostRecentProducts = result.Data!.MostRecentProducts
                    .Select(p => new ProductCardResponseVM
                    {
                        ProductId = p.ProductId,
                        ShopId = p.ShopId,
                        ProductName = p.ProductName,
                        ShopName = p.ShopName,
                        Price = p.Price,
                        ExpectedDays = p.ExpectedDays,
                        AverageRating = p.AverageRating,
                        ReviewCount = p.ReviewCount,
                        RelativePath = p.RelativePath,
                        AltText = p.AltText
                    }).ToList()
            };

            return Ok(response);
        }
        [HttpGet("CategoryProducts")]
        public async Task<ActionResult> GetCategoryProducts(Guid categoryId, int pageNumber, int pageSize, CancellationToken ct)
        {
            var result = await mediator.Send(new GetPublicProductsQuery(new PublicProductsCriteria { CategoryId = categoryId }, pageNumber, pageSize),ct);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get products", HttpContext.Request.Path);

            var response = result.Data.ToPagedResponseVM(product => new ProductCardResponseVM
            {
                ProductId = product.ProductId,
                ShopId = product.ShopId,
                ProductName = product.ProductName,
                ShopName = product.ShopName,
                Price = product.Price,
                ExpectedDays = product.ExpectedDays,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                RelativePath = product.RelativePath,
                AltText = product.AltText
            });

            return Ok(response);     
        }
        [HttpGet("ShopProducts")]
        public async Task<ActionResult> GetShopProducts(Guid shopId, int pageNumber, int pageSize, CancellationToken ct)
        {
            var result = await mediator.Send(new GetPublicProductsQuery(new PublicProductsCriteria { ShopId = shopId }, pageNumber, pageSize), ct);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get products", HttpContext.Request.Path);

            var response = result.Data.ToPagedResponseVM(product => new ProductCardResponseVM
            {
                ProductId = product.ProductId,
                ShopId = product.ShopId,
                ProductName = product.ProductName,
                ShopName = product.ShopName,
                Price = product.Price,
                ExpectedDays = product.ExpectedDays,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                RelativePath = product.RelativePath,
                AltText = product.AltText
            });

            return Ok(response);
        }

        /// <summary>
        /// Public category browse, with subcategories nested — the only source of
        /// subcategories that isn't Admin-only. Reuses the admin dashboard's own
        /// query; nothing in it is admin-specific.
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new GetCategoryManagementDashboardQuery(SearchTerm: null, PageNumber: 1, PageSize: 100),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get categories", HttpContext.Request.Path);

            var response = result.Data!.Items.Select(c => new CategoryResponseVM
            {
                Id = c.Id,
                Name = c.CategoryName,
                Description = c.CategoryDescription,
                ImageUrl = c.CategoryImage,
                CreatedAt = c.CreatedAt,
                SubCategories = c.Subcategories.Select(sc => new SubCategoryResponseVM
                {
                    Id = sc.Id,
                    Name = sc.SubcategoryName,
                    CreatedAt = sc.CreatedAt
                }).ToList()
            });

            return Ok(response);
        }

        /// <summary>
        /// Storefront product search/filter/sort — backs the products page's
        /// category, subcategory, search and sort controls in one call.
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult> GetProducts([FromQuery] GetPublicProductsCardsRequestVM request, CancellationToken cancellationToken)
        {
            var criteria = new PublicProductsCriteria
            {
                ShopId = request.ShopId,
                CategoryId = request.CategoryId,
                SubCategoryId = request.SubCategoryId,
                SearchTerm = request.SearchTerm,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection ?? Application.Shared.SortDirection.Desc
            };

            var result = await mediator.Send(
                new GetPublicProductsQuery(criteria, request.PageNumber, request.PageSize),
                cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get products", HttpContext.Request.Path);

            var response = result.Data!.ToPagedResponseVM(product => new ProductCardResponseVM
            {
                ProductId = product.ProductId,
                ShopId = product.ShopId,
                ProductName = product.ProductName,
                ShopName = product.ShopName,
                Price = product.Price,
                ExpectedDays = product.ExpectedDays,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                RelativePath = product.RelativePath,
                AltText = product.AltText
            });

            return Ok(response);
        }

        [HttpGet("products/{productId:guid}")]
        public async Task<ActionResult<ProductDetailsResponseVM>> GetProductDetails(Guid productId,CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetPublicProductDetailsQuery(productId), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Product not found.", HttpContext.Request.Path);

            var data = result.Data!;

            var response = new ProductDetailsResponseVM
            {
                ProductId = data.ProductId,
                ShopId = data.ShopId,
                ShopName = data.ShopName,
                ProductName = data.ProductName,
                AverageRating = data.AverageRating,
                ReviewCount = data.ReviewCount,
                Price = data.Price,
                ExpectedDays = data.ExpectedDays,
                Description = data.Description,
                Reviews = data.Reviews.Select(r => new ProductReviewResonseVM
                {
                    ReviewerName = r.ReviewerName,
                    ReviewTitle = r.ReviewTitle,
                    ReviewContent = r.ReviewContent,
                    Rating = r.Rating
                }).ToList(),
                Images = data.Images.Select(i => new ProductImageResponseVM
                {
                    Id = i.Id,
                    Url = i.Url,
                    AltText = i.AltText,
                    SortOrder = i.SortOrder,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };

            return Ok(response);
        }

        /// <summary>
        /// Public reviews for any target. TargetId is a shared polymorphic column, so
        /// TargetType is required — without it the query would mix product, shop and
        /// buyer reviews that happen to share an id.
        /// </summary>
        [HttpGet("reviews")]
        public async Task<ActionResult> GetReviews(
            [FromQuery] ReviewTargetType targetType,
            [FromQuery] Guid targetId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var criteria = new ReviewsCriteria
            {
                TargetType = targetType,
                TargetId = targetId,
                Status = ReviewStatus.Approved
            };

            var result = await mediator.Send(
                new GetReviewsQuery(criteria, pageNumber, pageSize), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get the reviews", HttpContext.Request.Path);

            var paged = result.Data!.ToPagedResponseVM(r => new ProductReviewResonseVM
            {
                ReviewerName = r.ReviewerName,
                ReviewTitle = r.ReviewTitle,
                ReviewContent = r.ReviewContent,
                Rating = r.Rating
            });

            return Ok(paged);
        }

        [HttpGet("reviews/summary")]
        public async Task<ActionResult> GetReviewSummary(
            [FromQuery] ReviewTargetType targetType,
            [FromQuery] Guid targetId,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(
                new GetReviewSummaryQuery(targetType, targetId, ReviewStatus.Approved), cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem("Faild to get the review summary", HttpContext.Request.Path);

            return Ok(new ReviewSummaryResponseVM
            {
                AverageRating = result.Data!.AverageRating,
                ReviewCount = result.Data.ReviewCount
            });
        }
    }
}
