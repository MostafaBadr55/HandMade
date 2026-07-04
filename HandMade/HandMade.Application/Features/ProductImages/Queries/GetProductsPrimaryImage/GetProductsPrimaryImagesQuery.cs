using HandMade.Application.Features.ProductImages.Queries.GetProductsPrimaryImage.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Queries.GetProductsPrimaryImage
{
    public record GetProductsPrimaryImagesQuery(IEnumerable<Guid> productIds) : 
        IRequest<RequestResult<Dictionary<Guid, PrimaryImageDTO>>>;

    public class GetProductsPrimaryImagesQueryHandler(IUnitOfWork unitOfWork, IQueryableExecutor executor) : IRequestHandler<GetProductsPrimaryImagesQuery, RequestResult<Dictionary<Guid, PrimaryImageDTO>>>
    {
       
        public async Task<RequestResult<Dictionary<Guid, PrimaryImageDTO>>> Handle(GetProductsPrimaryImagesQuery request, CancellationToken cancellationToken)
        {
            if (!request.productIds.Any())
                return RequestResult<Dictionary<Guid, PrimaryImageDTO>>.Success([]);

            var query = unitOfWork.GetRepository<ProductImage>()
                                           .Get(i => request.productIds.Contains(i.ProductId) && i.IsPrimary)
                                           .Select(i=> new PrimaryImageDTO
                                           {
                                               ProductId = i.ProductId,
                                               RelativePath = i.Url,
                                               AltText = i.AltText
                                           });

            List<PrimaryImageDTO> primaryImages = await executor.ToListAsync(query, cancellationToken);

            Dictionary<Guid, PrimaryImageDTO> mappedPrimaryImages = primaryImages.ToDictionary(i => i.ProductId, i => i);

            return RequestResult<Dictionary<Guid, PrimaryImageDTO>>.Success(mappedPrimaryImages);
        }
    }
}
