using HandMade.Application.Features.ProductImages.Commands.CreateProductImages.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Commands.CreateProductImages
{
    public record CreateProductImagesCommand(Guid ProductId,List<CreateProductImageDto> Images) : IRequest<RequestResult<bool>>;

    public class CreateProductImagesCommandHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<CreateProductImagesCommand, RequestResult<bool>>
    {
        
        public async Task<RequestResult<bool>> Handle(
            CreateProductImagesCommand request,
            CancellationToken cancellationToken)
        {
            var images = request.Images.Select(i => new ProductImage
            {
                ProductId = request.ProductId,
                Url = i.RelativePath,
                AltText = i.AltText,
                IsPrimary = i.IsPrimary,
                SortOrder = i.SortOrder,
            }).ToList();

             _unitOfWork.GetRepository<ProductImage>().AddRange(images);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }


}
