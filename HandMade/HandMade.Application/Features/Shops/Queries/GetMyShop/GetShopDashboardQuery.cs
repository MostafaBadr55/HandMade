using HandMade.Application.Features.ProductImages.Queries.GetProductImages;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard;
using HandMade.Application.Features.Products.Queries.GetProductsForSellerDashboard.DTOs;
using HandMade.Application.Features.Shops.Queries.GetMyShop.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Queries.GetShopDashboard
{
    public record GetShopDashboardQuery(Guid userId) : IRequest<RequestResult<MyShopDTO>>;

    public class GetShopDashboardQueryHandler(IUnitOfWork _unitOfWork, IMediator _mediator, IUrlBuilder _urlBuilder)
        : IRequestHandler<GetShopDashboardQuery, RequestResult<MyShopDTO>>
    {
        public async Task<RequestResult<MyShopDTO>> Handle(GetShopDashboardQuery request, CancellationToken cancellationToken)
        {
            //Check if the user has a shop
            MyShopDTO? shop = _unitOfWork.GetRepository<Shop>()
                                  .Get(s => s.OwnerUserId == request.userId)
                                  .Select(s => new MyShopDTO
                                  {
                                      Id = s.Id,
                                      Name = s.Name,
                                      Status = s.Status,
                                      Description = s.Description,
                                      ImageUrl = _urlBuilder.BuildAbsoluteUrl(s.ImageUrl),
                                      RejectionMessage = s.RejectionMessage
                                  })
                                  .FirstOrDefault();

            if (shop is null)
                return RequestResult<MyShopDTO>.Failed(ErrorCode.ShopNotFound);
            //Get shops active and inactive product
            PagedResult<ProductForSellerDTO>? activeProducts = (await _mediator.Send(
                new GetProductsForSellerDashboardQuery(
                    new ProductsForSellerCriteria { Status = ProductStatus.Active },request.userId, shop.Id, 
                    pageNumber: 1, pageSize: 10, 
                    cancellationToken))).Data;

            PagedResult<ProductForSellerDTO>? inactiveProducts = (await _mediator.Send(new GetProductsForSellerDashboardQuery(
                    new ProductsForSellerCriteria { Status = ProductStatus.Active }, request.userId, shop.Id,
                    pageNumber: 1, pageSize: 10,
                    cancellationToken))).Data;
            
            //Map active and inactive products to the DTO
            shop.ActiveProductDtos = activeProducts.Items;
            shop.InActiveProductDtos = inactiveProducts.Items;

            //return shop with active and inactive product
            return RequestResult<MyShopDTO>.Success(shop); 

        }
    }


}
