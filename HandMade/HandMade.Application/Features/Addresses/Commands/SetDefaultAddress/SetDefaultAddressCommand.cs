using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Commands.SetDefaultAddress
{
    public record SetDefaultAddressCommand(Guid UserId, Guid AddressId) : IRequest<RequestResult<bool>>;

    public class SetDefaultAddressCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<SetDefaultAddressCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            SetDefaultAddressCommand request,
            CancellationToken cancellationToken)
        {
            var addressRepo = _unitOfWork.GetRepository<Address>();

            // Load the whole set tracked: promoting one address demotes the others,
            // and both halves must land in the same SaveChanges.
            var addresses = await _executor.ToListAsync(
                addressRepo.GetRangeWithTracking(a => a.UserId == request.UserId),
                cancellationToken);

            var target = addresses.FirstOrDefault(a => a.Id == request.AddressId);

            if (target is null)
                return RequestResult<bool>.Failed(ErrorCode.AddressNotFound);

            foreach (var address in addresses)
                address.IsDefault = address.Id == request.AddressId;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
