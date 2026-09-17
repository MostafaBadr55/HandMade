using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Commands.CreateAddress
{
    public record CreateAddressCommand(
        Guid UserId,
        string Label,
        string DetailedAddress,
        bool IsDefault) : IRequest<RequestResult<Guid>>;

    public class CreateAddressCommandHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<CreateAddressCommand, RequestResult<Guid>>
    {
        public async Task<RequestResult<Guid>> Handle(
            CreateAddressCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Label))
                return RequestResult<Guid>.Failed(ErrorCode.LabelMustBeProvided);

            if (string.IsNullOrWhiteSpace(request.DetailedAddress))
                return RequestResult<Guid>.Failed(ErrorCode.DetailedAddressNotProvided);

            var addressRepo = _unitOfWork.GetRepository<Address>();

            var existing = await _executor.ToListAsync(
                addressRepo.GetRangeWithTracking(a => a.UserId == request.UserId),
                cancellationToken);

            // The first address a user creates is always their default — otherwise
            // they could end up with no default at all and nothing to ship to.
            var shouldBeDefault = request.IsDefault || existing.Count == 0;

            if (shouldBeDefault)
            {
                foreach (var address in existing.Where(a => a.IsDefault))
                    address.IsDefault = false;
            }

            var newAddress = new Address
            {
                UserId = request.UserId,
                Label = request.Label,
                DetailedAddress = request.DetailedAddress,
                IsDefault = shouldBeDefault
            };

            addressRepo.Add(newAddress);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<Guid>.Success(newAddress.Id);
        }
    }
}
