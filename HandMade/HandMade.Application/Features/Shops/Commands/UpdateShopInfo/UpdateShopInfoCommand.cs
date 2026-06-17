using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Commands.UpdateShopInfo
{
    public record UpdateShopInfoCommand(Guid requestingUserId, Guid shopId, string shopName, string shopDescription, string imageRelativePath) : IRequest<RequestResult<bool>>;

    internal class UpldateShopInfoCommandHandler(IUnitOfWork _unitOfWork, IMediator _mediator)
        : IRequestHandler<UpdateShopInfoCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateShopInfoCommand request, CancellationToken cancellationToken)
        {
            bool ownsThisShop = (await _mediator.Send(new ValidateShopOwnershipQuery(request.requestingUserId, request.shopId))).Data;

            if (!ownsThisShop)
                return RequestResult<bool>.Failed(ErrorCode.UserDoesNotOwnThisShop);
            var shopRepo = _unitOfWork.GetRepository<Shop>();

            var trackedShop = shopRepo.GetByIdWithTracking(request.shopId)
                                      .FirstOrDefault();

            trackedShop.Name = request.shopName;
            trackedShop.Description = request.shopDescription;
            trackedShop.ImageUrl = request.imageRelativePath;
            trackedShop.UpdatedAt = DateTime.UtcNow;

            shopRepo.Update(trackedShop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
