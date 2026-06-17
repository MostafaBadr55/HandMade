using HandMade.Application.Features.ProductImages.Queries.GetProductsImages;
using HandMade.Application.Features.ProductImages.Queries.GetProductsImages.DTO;
using HandMade.Application.Features.Products.Queries.FilterHelpers;
using HandMade.Application.Features.Products.Queries.GetProductsForAdmin.DTOs;
using HandMade.Application.Helpers;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Queries.GetProductsForAdmin
{
    public record GetProductsForAdminQuery(ProductQueryCriteria criteria, int pageNumber, int pageSize, CancellationToken ct) : IRequest<RequestResult<PagedResult<ProductDetailsDTO>>>;

    internal class GetProductsForAdminQueryHandler(IMediator _mediator, IUnitOfWork unitOfWork, IQueryableExecutor _executor) : IRequestHandler<GetProductsForAdminQuery, RequestResult<PagedResult<ProductDetailsDTO>>>
    {
        public async Task<RequestResult<PagedResult<ProductDetailsDTO>>> Handle(GetProductsForAdminQuery request, CancellationToken cancellationToken)
        {
            ProductsForAdminQuerySpecification spec = new ProductsForAdminQuerySpecification(request.criteria);

            PagedResult<ProductDetailsDTO> products = await unitOfWork.GetRepository<Product>()
                                           .GetAll()
                                           .ApplySpecification(spec)
                                           .Select(p => new ProductDetailsDTO
                                           {
                                               Id = p.Id,
                                               ShopName = p.Shop.Name,
                                               Title = p.Title,
                                               Price = p.Price,
                                               Status = p.Status,
                                               IsPublished = p.IsPublished
                                           })
                                           .ToPagedResultAsync(_executor, request.pageNumber, request.pageSize,request.ct);


            if (products.TotalCount == 0)
                return RequestResult<PagedResult<ProductDetailsDTO>>.Success(products);

            var productsIds = products.Items.Select(p => p.Id).ToList();
            var productImages = (await _mediator.Send(new GetProductsImagesQuery(productsIds))).Data;

            foreach (var product in products.Items)
                product.Images = productImages?.GetValueOrDefault(product.Id)?? [];

            return RequestResult<PagedResult<ProductDetailsDTO>>.Success(products);

        }
    }
}
