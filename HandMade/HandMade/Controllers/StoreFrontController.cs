using HandMade.Application.Features.HomePage.Queries;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard;
using HandMade.Application.Features.Products.Queries.GetPublicProductCard.DTOs;
using HandMade.Application.Features.Products.Queries.GetPublicProductDetails;
using HandMade.Application.Interfaces;
using HandMade.Helpers;
using HandMade.ViewModels.ProductImage;
using HandMade.ViewModels.Products;
using HandMade.ViewModels.Review;
using HandMade.ViewModels.StoreFront;
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
                result.ErrorCode.ToProblem("Faild to get products");

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
                result.ErrorCode.ToProblem("Faild to get products");

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
    }
}
