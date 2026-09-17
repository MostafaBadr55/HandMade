using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Addresses.Queries.ValidateAddressOwnership
{
    /// <summary>
    /// Reusable guard for anything that accepts a shipping address from the caller.
    /// Returns false rather than failing, so the caller picks the ErrorCode that
    /// fits its own context.
    /// </summary>
    public record ValidateAddressOwnershipQuery(Guid UserId, Guid AddressId) : IRequest<RequestResult<bool>>;

    public class ValidateAddressOwnershipQueryHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<ValidateAddressOwnershipQuery, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            ValidateAddressOwnershipQuery request,
            CancellationToken cancellationToken)
        {
            var owned = await _unitOfWork
                .GetRepository<Address>()
                .AnyAsync(a => a.Id == request.AddressId && a.UserId == request.UserId, cancellationToken);

            return RequestResult<bool>.Success(owned);
        }
    }
}
