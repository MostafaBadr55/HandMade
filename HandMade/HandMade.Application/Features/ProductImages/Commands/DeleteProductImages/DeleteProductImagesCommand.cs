using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.ProductImages.Commands.DeleteProductImages
{
    public record DeleteProductImagesCommand(Guid ProductId)
    : IRequest<RequestResult<bool>>;

    public class DeleteProductImagesCommandHandler(IUnitOfWork _unitOfWork, IStorageService _storageService, IQueryableExecutor executer)
    : IRequestHandler<DeleteProductImagesCommand, RequestResult<bool>>
    { 

        
        public async Task<RequestResult<bool>> Handle(DeleteProductImagesCommand request,CancellationToken cancellationToken)
        {
            var imageRepo = _unitOfWork.GetRepository<ProductImage>();
            var query = imageRepo.GetRangeWithTracking(i => i.ProductId == request.ProductId);


            var images = await executer.ToListAsync(query);   

            if (images.Count == 0)
                return RequestResult<bool>.Success(true); // nothing to delete, not an error

            // Delete physical files first — storage is not transactional
            
            foreach (var image in images)
                await _storageService.DeleteAsync(image.Url, cancellationToken);

            // Mark all image records as deleted
            imageRepo.DeleteRange(images);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
