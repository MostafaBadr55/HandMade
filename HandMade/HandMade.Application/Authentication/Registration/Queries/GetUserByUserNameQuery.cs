using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Authentication.Registration.Queries
{
    public record GetUserByUserNameQuery(string username) : IRequest<RequestResult<User?>>;

    public class GetUserByUserNameQueryHandler : IRequestHandler<GetUserByUserNameQuery, RequestResult<User?>>
    {
        private readonly IAuthRepository _authRepository;
        public GetUserByUserNameQueryHandler(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<RequestResult<User?>> Handle(GetUserByUserNameQuery request, CancellationToken cancellationToken)
        {
            User? user = await _authRepository.GetUserByUsernameAsync(request.username);
            return user is null ? RequestResult<User?>.Faild(ErrorCode.UserNotFound) : RequestResult<User?>.Success(user);
        }
    }

}
