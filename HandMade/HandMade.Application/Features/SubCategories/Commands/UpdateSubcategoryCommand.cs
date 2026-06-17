using HandMade.Application.Features.Categories.Queries.ValidateCategoryExists;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.SubCategories.Commands
{
    public record UpdateSubcategoryCommand(Guid subcategoryId, Guid categoryId, string name) : IRequest<RequestResult<bool>>;

    internal class UpdateSubcategoryCommandHandler(IUnitOfWork _unitOfWork, IMediator _mediator)
        : IRequestHandler<UpdateSubcategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateSubcategoryCommand request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<SubCategory>();

            // Step 1 — subcategory must exist
            var subCategory =  repo.GetByIdWithTracking(request.subcategoryId).FirstOrDefault();
            if (subCategory is null)
                return RequestResult<bool>.Failed(ErrorCode.SubCategoryNotFound);

            // Step 2 — validate existing category
            var categoryCheck = await _mediator.Send(
                new ValidateCategoryExistsQuery(request.categoryId),
                cancellationToken);

            if (!categoryCheck.IsSuccess)
                return RequestResult<bool>.Failed(categoryCheck.ErrorCode);

            // Step 3 — name uniqueness within target category (excluding self)
            bool nameConflict = repo
                .GetAll()
                .Any(sc => sc.CategoryId == request.categoryId
                        && sc.Name == request.name
                        && sc.Id != request.subcategoryId);

            if (nameConflict)
                return RequestResult<bool>.Failed(ErrorCode.SubCategoryNameAlreadyExistsInCategory);

            
            subCategory.CategoryId = request.categoryId;
            subCategory.Name = request.name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
