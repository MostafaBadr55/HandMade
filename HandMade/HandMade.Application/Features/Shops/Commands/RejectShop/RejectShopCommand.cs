using HandMade.Application.Features.Shops.Queries.GetShopStatus;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Commands.RejectShop
{
    public record RejectShopCommand(Guid shopId, string rejectionMessage) : IRequest<RequestResult<bool>>;

    internal class RejectShopCommandHandler(IMediator mediator, IUnitOfWork unitOfWork) : IRequestHandler<RejectShopCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(RejectShopCommand request, CancellationToken cancellationToken)
        {
            var shopStatus = await mediator.Send(new GetShopStatusQuery(request.shopId), cancellationToken);

            if (!shopStatus.IsSuccess)
                return RequestResult<bool>.Failed(shopStatus.ErrorCode);

            // Check if the Shop status is pending as it is the only case to approve it.
            if (shopStatus.Data != ShopStatus.Pending)
                return RequestResult<bool>.Failed(ErrorCode.ShopNotPending);

            // Load tracked shop and update its status
            var shop = unitOfWork
                .GetRepository<Shop>()
                .GetByIdWithTracking(request.shopId)
                .FirstOrDefault();

            shop!.Status = ShopStatus.Active;
            shop.RejectionMessage = request.rejectionMessage;
            shop!.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);

        }
    }
}
