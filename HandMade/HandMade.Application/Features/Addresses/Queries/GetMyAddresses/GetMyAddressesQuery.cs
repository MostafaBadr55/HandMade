using HandMade.Application.Features.Addresses.Queries.GetMyAddresses.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Queries.GetMyAddresses
{
    public record GetMyAddressesQuery(Guid UserId) : IRequest<RequestResult<List<AddressDTO>>>;

    public class GetMyAddressesQueryHandler(IUnitOfWork _unitOfWork, IQueryableExecutor _executor)
        : IRequestHandler<GetMyAddressesQuery, RequestResult<List<AddressDTO>>>
    {
        public async Task<RequestResult<List<AddressDTO>>> Handle(GetMyAddressesQuery request,
            CancellationToken cancellationToken)
        {
            var addresses = await _executor.ToListAsync(
                _unitOfWork.GetRepository<Address>()
                    .Get(a => a.UserId == request.UserId)
                    .OrderByDescending(a => a.IsDefault)
                    .ThenBy(a => a.Label)
                    .Select(a => new AddressDTO
                    {
                        Id = a.Id,
                        Label = a.Label,
                        DetailedAddress = a.DetailedAddress,
                        IsDefault = a.IsDefault
                    }),
                cancellationToken);

            return RequestResult<List<AddressDTO>>.Success(addresses);
        }
    }
}
