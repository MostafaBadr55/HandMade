using HandMade.Application.Features.ProductImages.Queries.GetProductImages.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Queries.GetProductImages
{
    public record GetProductImagesQuery(Guid productId) : IRequest<RequestResult<List<ProductImageDTO>>>;

    internal class GetProductImagesQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor, IUrlBuilder _urlBuilder) : IRequestHandler<GetProductImagesQuery, RequestResult<List<ProductImageDTO>>>
    {
        public async Task<RequestResult<List<ProductImageDTO>>> Handle(GetProductImagesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ProductImageDTO> query =  _unitOfWork.GetRepository<ProductImage>()
                                     .Get(i => i.ProductId == request.productId)
                                     .Select(i => new ProductImageDTO
                                       {
                                           Id = i.Id,
                                           Url = _urlBuilder.BuildAbsoluteUrl(i.Url),
                                           AltText = i.AltText,
                                           IsPrimary = i.IsPrimary,
                                           SortOrder = i.SortOrder
                                       });
            List<ProductImageDTO> images = await _executor.ToListAsync(query, cancellationToken);
            return RequestResult<List<ProductImageDTO>>.Success(images);
        }
    }

}
