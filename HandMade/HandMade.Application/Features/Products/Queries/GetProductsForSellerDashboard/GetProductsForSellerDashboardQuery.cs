using HandMade.Application.Features.ProductImages.Queries.GetProductsImages;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard
{
    public record GetProductsForSellerDashboardQuery(ProductsForSellerCriteria criteria, Guid ownerId, Guid shopId, int pageNumber, int pageSize, CancellationToken ct) : IRequest<RequestResult<PagedResult<ProductForSellerDTO>>>;

    internal class GetProductsForSellerDashboardQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor, IMediator _mediator)
        : IRequestHandler<GetProductsForSellerDashboardQuery, RequestResult<PagedResult<ProductForSellerDTO>>>
    {
        public async Task<RequestResult<PagedResult<ProductForSellerDTO>>> Handle(GetProductsForSellerDashboardQuery request, CancellationToken cancellationToken)
        {
            //validate shop ownership
            bool shopOwnership = (await _mediator.Send(new ValidateShopOwnershipQuery(request.ownerId, request.shopId))).Data;
            if (!shopOwnership)
                return RequestResult<PagedResult<ProductForSellerDTO>>.Failed(ErrorCode.UserDoesNotOwnThisShop);

            ProductsForSellerQuerySpecification specs = new ProductsForSellerQuerySpecification(request.criteria);

            var shopProducts = await _unitOfWork.GetRepository<Product>()
                                   .Get(p => p.ShopId == request.shopId)
                                   .ApplySpecification(specs)
                                   .Select(p => new ProductForSellerDTO
                                   {
                                      Id = p.Id,
                                      Title = p.Title,
                                      Status = p.Status,
                                      IsPublished = p.IsPublished,
                                      Price = p.Price,
                                   })
                                   .ToPagedResultAsync(_executor, request.pageNumber, request.pageSize);

            var productsIds = shopProducts.Items.Select(p => p.Id).ToList();
            var productImages = (await _mediator.Send(new GetProductsImagesQuery(productsIds))).Data;

            foreach (var product in shopProducts.Items)
                product.Images = productImages?.GetValueOrDefault(product.Id) ?? [];

            return RequestResult<PagedResult<ProductForSellerDTO>>.Success(shopProducts);
        }
    }

}
