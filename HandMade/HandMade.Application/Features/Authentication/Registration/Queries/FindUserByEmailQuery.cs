using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.CQRS.Authentication.Registration.Queries
{
    public record FindUserByEmailQuery(string email) : IRequest<RequestResult<User>>;

    public class FindUserByEmailQueryHandler: IRequestHandler<FindUserByEmailQuery,RequestResult<User?>>
    {
        private readonly IAccountServices _authRepository;
        public FindUserByEmailQueryHandler(IAccountServices authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<RequestResult<User?>> Handle(FindUserByEmailQuery request, CancellationToken cancellationToken)
        {
            User? user = await _authRepository.GetUserByEmailAsync(request.email);
            return user is null? RequestResult<User>.Failed(ErrorCode.UserNotFound): RequestResult<User>.Success(user);

        }
    }

}
