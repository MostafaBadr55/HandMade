using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Categories.Queries.ValidateCategoryExists
{
    public record ValidateCategoryExistsQuery(Guid CategoryId) : IRequest<RequestResult<bool>>;

    public class ValidateCategoryExistsQueryHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<ValidateCategoryExistsQuery, RequestResult<bool>>
    {

        public async Task<RequestResult<bool>> Handle(
            ValidateCategoryExistsQuery request,
            CancellationToken cancellationToken)
        {
            var exists = _unitOfWork
                .GetRepository<Category>()
                .GetAll()
                .Any(c => c.Id == request.CategoryId);

            if (!exists)
                return RequestResult<bool>.Failed(ErrorCode.CategoryNotFound);

            return RequestResult<bool>.Success(true);
        }
    }
}
