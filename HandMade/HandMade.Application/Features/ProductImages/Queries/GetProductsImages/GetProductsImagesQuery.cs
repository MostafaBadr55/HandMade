using HandMade.Application.Features.ProductImages.Queries.GetProductImages.DTOs;
using HandMade.Application.Features.ProductImages.Queries.GetProductsImages.DTO;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Queries.GetProductsImages
{
    public record GetProductsImagesQuery(IEnumerable<Guid> productsIds) : IRequest<RequestResult<Dictionary<Guid, List<ProductImageDictionaryItemDTO>>>>;

    internal class GetProductsImagesQueryHandler(IUnitOfWork _unitOfWork, IUrlBuilder _urlBuilder, IQueryableExecutor _executer) : IRequestHandler<GetProductsImagesQuery, RequestResult<Dictionary<Guid, List<ProductImageDictionaryItemDTO>>>>
    {
        public async Task<RequestResult<Dictionary<Guid, List<ProductImageDictionaryItemDTO>>>> Handle(GetProductsImagesQuery request, CancellationToken cancellationToken)
        {
            var productsIdsList = request.productsIds;
            var query = _unitOfWork.GetRepository<ProductImage>()
                                   .Get(i => productsIdsList.Contains(i.ProductId))
                                   .OrderBy(i => i.ProductId)
                                   .ThenByDescending(i => i.IsPrimary)
                                   .ThenBy(i=> i.SortOrder)
                                   .Select(pI => new ProductImageDictionaryItemDTO
                                   {
                                       Id = pI.Id,
                                       ProductId = pI.ProductId,
                                       Url = _urlBuilder.BuildAbsoluteUrl(pI.Url),
                                       AltText = pI.AltText,
                                       IsPrimary = pI.IsPrimary,
                                       SortOrder = pI.SortOrder
                                   });

            List<ProductImageDictionaryItemDTO> images = await _executer.ToListAsync(query, cancellationToken);

            Dictionary<Guid, List<ProductImageDictionaryItemDTO>> mappedProductImages = 
                images.GroupBy(i=> i.ProductId)
                      .ToDictionary(g=> g.Key, g => g.ToList());

            return RequestResult<Dictionary<Guid, List<ProductImageDictionaryItemDTO>>>.Success(mappedProductImages);
        }
    }
}
