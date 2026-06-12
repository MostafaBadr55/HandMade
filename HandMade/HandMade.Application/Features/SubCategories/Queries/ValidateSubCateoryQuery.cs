using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.SubCategories.Queries
{
    public record ValidateSubCategoryExistsQuery(Guid SubCategoryId,Guid CategoryId): IRequest<RequestResult<bool>>;

    public class ValidateSubCategoryExistsQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<ValidateSubCategoryExistsQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateSubCategoryExistsQuery request,
            CancellationToken cancellationToken)
        {
            var exists = _unitOfWork
                .GetRepository<SubCategory>()
                .GetAll()
                .Any(sc => sc.Id == request.SubCategoryId
                        && sc.CategoryId == request.CategoryId);

            if (!exists)
                return RequestResult<bool>.Failed(ErrorCode.SubCategoryNotFound);

            return RequestResult<bool>.Success(true);
        }
    }
}
