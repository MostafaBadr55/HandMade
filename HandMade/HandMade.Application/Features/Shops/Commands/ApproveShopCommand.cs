using HandMade.Application.Features.Shops.Queries.GetShopStatus;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.DomainEnums;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Shops.Commands
{
    public record ApproveShopCommand(Guid ShopId) : IRequest<RequestResult<bool>>;

    internal sealed class ApproveShopCommandHandler(IMediator mediator, IUnitOfWork unitOfWork)
    : IRequestHandler<ApproveShopCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ApproveShopCommand request,
            CancellationToken cancellationToken)
        {
            
            var statusResult = await mediator.Send(new GetShopStatusQuery(request.ShopId), cancellationToken);

            if (!statusResult.IsSuccess)
                return RequestResult<bool>.Failed(statusResult.ErrorCode);

            // Check if the Shop status is pending as it is the only case to approve it.
            if (statusResult.Data != ShopStatus.Pending)
                return RequestResult<bool>.Failed(ErrorCode.ShopNotPending);

            // Load tracked shop and update its status
            var shop = unitOfWork
                .GetRepository<Shop>()
                .GetById(request.ShopId)
                .FirstOrDefault();

            shop!.Status = ShopStatus.Active;
            shop!.UpdatedAt = DateTime.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
