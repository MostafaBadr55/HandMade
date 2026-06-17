using HandMade.Application.Features.Shops.Queries.ValidateShopOwnership;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Commands.UpdateShopActivityStatus
{
    public record UpdateShopActivityStatusCommand(Guid ownerId, Guid shopId, ShopStatus status) : IRequest<RequestResult<bool>>;

    public class UpdateShopActivityStatusCommandHandler(IUnitOfWork _unitOfWork, IMediator _mediator)
        : IRequestHandler<UpdateShopActivityStatusCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(UpdateShopActivityStatusCommand request, CancellationToken cancellationToken)
        {
            var shopOwnership = (await _mediator.Send(new ValidateShopOwnershipQuery(request.ownerId, request.shopId))).Data;

            if (!shopOwnership)
                return RequestResult<bool>.Failed(ErrorCode.UserDoesNotOwnThisShop);

            var shopRepo = _unitOfWork.GetRepository<Shop>();

            var trackedShop = shopRepo.GetByIdWithTracking(request.shopId).FirstOrDefault();

            trackedShop.Status = request.status;
            trackedShop.UpdatedAt = DateTime.Now;

            shopRepo.Update(trackedShop);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);

        }
    }
}
