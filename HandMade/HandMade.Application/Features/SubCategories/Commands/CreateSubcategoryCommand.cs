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
    public record CreateSubcategoryCommand(Guid categoryId, string name) : IRequest<RequestResult<bool>>;

    internal class CreateSubcategoryCommandHandler(IUnitOfWork _unitOfWork, IMediator _mediator)
        : IRequestHandler<CreateSubcategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(CreateSubcategoryCommand request, CancellationToken cancellationToken)
        {
            // Step 1 — validate category exists (reusable step)
            var categoryCheck = await _mediator.Send(
                new ValidateCategoryExistsQuery(request.categoryId),
                cancellationToken);

            if (!categoryCheck.IsSuccess)
                return RequestResult<bool>.Failed(categoryCheck.ErrorCode);

            // Step 2 — uniqueness within category
            var subCatRepo = _unitOfWork.GetRepository<SubCategory>();

            bool nameExists = subCatRepo
                .GetAll()
                .Any(sc => sc.CategoryId == request.categoryId && sc.Name == request.name);

            if (nameExists)
                return RequestResult<bool>.Failed(ErrorCode.SubCategoryNameAlreadyExistsInCategory);

            // Step 3 — persist
            var subCategory = new SubCategory
            {
                CategoryId = request.categoryId,
                Name = request.name
            };

            subCatRepo.Add(subCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }


    }
    
}
