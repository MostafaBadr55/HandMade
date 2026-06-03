using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace HandMade.Application.Addresses.Commands
{
    public record CreateAddressForNewUserCommand(Guid userId, string label, string detailedAddress) : IRequest<RequestResult<bool>>;

    public class CreateAddressForNewUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateAddressForNewUserCommand, RequestResult<bool>>
    {

        public async Task<RequestResult<bool>> Handle(CreateAddressForNewUserCommand request, CancellationToken cancellationToken)
        {
            IGeneralRepository<Address> addressRepo = unitOfWork.GetRepository<Address>();

            if (String.IsNullOrEmpty(request.label))
                return RequestResult<bool>.Faild(ErrorCode.LabelMustBeProvided);
            if(String.IsNullOrEmpty(request.detailedAddress))
                return RequestResult<bool>.Faild(ErrorCode.DetailedAddressNotProvided);

            Address address = new Address()
            {
                UserId= request.userId,
                Label = request.label,
                DetailedAddress = request.detailedAddress,
                IsDefault = true
            };

            addressRepo.Add(address);
            return RequestResult<bool>.Success(true);
        }
    }
}
