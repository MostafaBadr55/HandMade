using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(Guid ProductId): IRequest<RequestResult<bool>>;

    public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, RequestResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task<RequestResult<bool>> Handle(
            DeleteProductCommand request,
            CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.GetRepository<Product>();

            var product = repo
                .GetByIdWithTracking(request.ProductId)
                .FirstOrDefault();

            if (product is null)
                return RequestResult<bool>.Failed(ErrorCode.ProductNotFound);

            repo.SoftDelete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
