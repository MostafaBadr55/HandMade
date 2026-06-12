using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Products.Commands
{
    public record CreateProductCommand(Guid ShopId,Guid CategoryId,Guid SubCategoryId,string Title,decimal Price) : IRequest<RequestResult<Guid>>;


    public class CreateProductCommandHandler(IUnitOfWork _unitOfWork)
    : IRequestHandler<CreateProductCommand, RequestResult<Guid>>
    {
    public async Task<RequestResult<Guid>> Handle(CreateProductCommand request,CancellationToken cancellationToken)
        {
            var product = new Product
            {
                ShopId = request.ShopId,
                CategoryId = request.CategoryId,
                SubCategoryId = request.SubCategoryId,
                Title = request.Title,
                Price = request.Price,
                IsPublished = false,
            };

                  _unitOfWork.GetRepository<Product>().Add(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(product.Id);
        }
    }
}
