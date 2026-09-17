using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Commands.UpdateAddress
{
    public record UpdateAddressCommand(
        Guid UserId,
        Guid AddressId,
        string Label,
        string DetailedAddress) : IRequest<RequestResult<bool>>;

    public class UpdateAddressCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<UpdateAddressCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            UpdateAddressCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Label))
                return RequestResult<bool>.Failed(ErrorCode.LabelMustBeProvided);

            if (string.IsNullOrWhiteSpace(request.DetailedAddress))
                return RequestResult<bool>.Failed(ErrorCode.DetailedAddressNotProvided);

            var addressRepo = _unitOfWork.GetRepository<Address>();

            var address = await _executor.FirstOrDefaultAsync(
                addressRepo.GetByIdWithTracking(request.AddressId),
                cancellationToken);

            if (address is null)
                return RequestResult<bool>.Failed(ErrorCode.AddressNotFound);

            if (address.UserId != request.UserId)
                return RequestResult<bool>.Failed(ErrorCode.AddressNotOwnedByUser);

            address.Label = request.Label;
            address.DetailedAddress = request.DetailedAddress;

            addressRepo.Update(address);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
