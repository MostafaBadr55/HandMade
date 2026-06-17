using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid cateoryId) : IRequest<RequestResult<bool>>;

    internal class DeleteCategoryCommandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<DeleteCategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var catRepo = _unitOfWork.GetRepository<Category>();

            var category = catRepo.GetByIdWithTracking(request.cateoryId).FirstOrDefault();

            if(category is null)
                return RequestResult<bool>.Failed(ErrorCode.CategoryNotFound);

            catRepo.SoftDelete(category);
            await _unitOfWork.SaveChangesAsync();

            return RequestResult<bool>.Success(true);
        }
    }
}
