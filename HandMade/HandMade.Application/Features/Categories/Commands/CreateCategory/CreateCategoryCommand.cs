using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string categoryName, string categoryDescription, string imageRelativePath)
        : IRequest<RequestResult<bool>>;

    public class CreateCategoryCommandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<CreateCategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var catRepo = _unitOfWork.GetRepository<Category>();

            var categoryExists = await catRepo.AnyAsync(c => c.Name == request.categoryName, cancellationToken);

            if (categoryExists)
                return RequestResult<bool>.Failed(ErrorCode.CategoryNameAlreadyExists);

            var category = new Category
            {
                Name = request.categoryName,
                Description = request.categoryDescription,
                ImageUrl = request.imageRelativePath
            };

            catRepo.Add(category);
            await _unitOfWork.SaveChangesAsync();

            return RequestResult<bool>.Success(true);
        }
    }
}
