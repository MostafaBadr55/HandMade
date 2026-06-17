using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Commands.CreateShop
{
    public record CreateShopCommand(Guid ownerId, string shopName, string description, string imageRelativePath) : IRequest<RequestResult<bool>>;

    public class CreateShopCommandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<CreateShopCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(CreateShopCommand request, CancellationToken cancellationToken)
        {
            var shopRepo = _unitOfWork.GetRepository<Shop>();
            //validate if this user already has a shop
            bool exists = shopRepo.Get(s => s.OwnerUserId == request.ownerId).Any();
            if (exists)
                return RequestResult<bool>.Failed(ErrorCode.ThisOwnerAlreadyHasAShop);

            var shop = new Shop { 
                OwnerUserId = request.ownerId ,
                Name = request.shopName,
                Description = request.description,
                ImageUrl = request.imageRelativePath,
                Status= Domain.DomainEnums.ShopStatus.Pending,
                CreatedAt = DateTime.UtcNow };

            shopRepo.Add(shop);
            await _unitOfWork.SaveChangesAsync();

            return RequestResult<bool>.Success(true);
        }
    }
}
