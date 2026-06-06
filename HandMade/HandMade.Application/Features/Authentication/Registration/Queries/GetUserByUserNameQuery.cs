using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.CQRS.Authentication.Registration.Queries
{
    public record GetUserByUserNameQuery(string username) : IRequest<RequestResult<User?>>;

    public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, RequestResult<User?>>
    {
        private readonly IAccountServices _authRepository;
        public GetUserByUserNameQueryHandler(IAccountServices authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<RequestResult<User?>> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            User? user = await _authRepository.GetUserByUsernameAsync(request.username);
            return user is null ? RequestResult<User?>.Failed(ErrorCode.UserNotFound) : RequestResult<User?>.Success(user);
        }
    }

}
