using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HandMade.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid categoryId, string categoryName, string categoryDescription, string imageRelativePath) : IRequest<RequestResult<bool>>;

    internal class UpdateCategoryCammandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<UpdateCategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var catRepo = _unitOfWork.GetRepository<Category>();

            //check if the category exists
            var category = catRepo.GetByIdWithTracking(request.categoryId).FirstOrDefault();
            if (category is null)
                return RequestResult<bool>.Failed(ErrorCode.CategoryNotFound);
            
            // Check if the updated name already exists
            bool nameConflict = await catRepo.AnyAsync(c => c.Name == request.categoryName && c.Id != request.categoryId, cancellationToken);

            if (nameConflict)
                return RequestResult<bool>.Failed(ErrorCode.CategoryNameAlreadyExists);

            //Update
            category.Name = request.categoryName;
            category.Description = request.categoryDescription;
            category.ImageUrl = request.imageRelativePath;
            
            await _unitOfWork.SaveChangesAsync();
            
            return RequestResult<bool>.Success(true);
        }
    }
}
