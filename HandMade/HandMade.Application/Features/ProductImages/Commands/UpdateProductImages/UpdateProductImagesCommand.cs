using HandMade.Application.Features.ProductImages.Commands.UpdateProductImages.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Commands.UpdateProductImages
{
    public record UpdateProductImagesCommand(Guid ProductId,List<UpdateProductImageDTO> Images) : IRequest<RequestResult<bool>>;

    public class UpdateProductImagesCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor, IStorageService _storageService)
    : IRequestHandler<UpdateProductImagesCommand, RequestResult<bool>>
    {
        
        public async Task<RequestResult<bool>> Handle(UpdateProductImagesCommand request,CancellationToken cancellationToken)
        {
            var existing = await _executor.ToListAsync(
                _unitOfWork.GetRepository<ProductImage>()
                    .GetAll()
                    .Where(pi => pi.ProductId == request.ProductId),
                cancellationToken);

            var incomingUrls = request.Images.Select(i => i.Url).ToHashSet();
            var existingUrls = existing.Select(e => e.Url).ToHashSet();

            // Removed by user → delete record + delete file
            var toDelete = existing.Where(e => !incomingUrls.Contains(e.Url)).ToList();
            _unitOfWork.GetRepository<ProductImage>().DeleteRange(toDelete);
            var urlsToDelete = toDelete.Select(e => e.Url).ToList();

            // Kept by user → update metadata only
            var toUpdate = existing.Where(e => incomingUrls.Contains(e.Url)).ToList();
            foreach (var image in toUpdate)
            {
                var incoming = request.Images.First(i => i.Url == image.Url);
                image.IsPrimary = incoming.IsPrimary;
                image.AltText = incoming.AltText;
                image.SortOrder = incoming.SortOrder;
            }

            // New URLs → insert record (file already on disk)
            var toInsert = request.Images
                .Where(i => !existingUrls.Contains(i.Url))
                .Select(i => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    Url = i.Url,
                    AltText = i.AltText,
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder
                }).ToList();

            _unitOfWork.GetRepository<ProductImage>().AddRange(toInsert);

            // Commit DB first, then clean up storage
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var url in urlsToDelete)
                await _storageService.DeleteAsync(url);

            return RequestResult<bool>.Success(true);
        }
    }
}
