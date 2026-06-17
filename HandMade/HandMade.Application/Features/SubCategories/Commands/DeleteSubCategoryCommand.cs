using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.SubCategories.Commands
{
    public record DeleteSubCategoryCommand(Guid Id) : IRequest<RequestResult<bool>>;

    public class DeleteSubCategoryCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSubCategoryCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            DeleteSubCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<SubCategory>();

            var subCategory =  repo.GetByIdWithTracking(request.Id).FirstOrDefault();
            if (subCategory is null)
                return RequestResult<bool>.Failed(ErrorCode.SubCategoryNotFound);

            repo.SoftDelete(subCategory);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
