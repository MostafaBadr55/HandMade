using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Commands.DeleteAddress
{
    public record DeleteAddressCommand(Guid UserId, Guid AddressId) : IRequest<RequestResult<bool>>;

    public class DeleteAddressCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<DeleteAddressCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            DeleteAddressCommand request,
            CancellationToken cancellationToken)
        {
            var addressRepo = _unitOfWork.GetRepository<Address>();

            var addresses = await _executor.ToListAsync(
                addressRepo.GetRangeWithTracking(a => a.UserId == request.UserId),
                cancellationToken);

            var target = addresses.FirstOrDefault(a => a.Id == request.AddressId);

            if (target is null)
                return RequestResult<bool>.Failed(ErrorCode.AddressNotFound);

            addressRepo.SoftDelete(target);

            // Never leave the user without a default: promote the oldest survivor.
            if (target.IsDefault)
            {
                var replacement = addresses
                    .Where(a => a.Id != target.Id)
                    .OrderBy(a => a.CreatedAt)
                    .FirstOrDefault();

                if (replacement is not null)
                    replacement.IsDefault = true;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
