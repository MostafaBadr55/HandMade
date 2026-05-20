using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Authentication.Registration.Queries
{
    public record GetUserByIdQuery(Guid userId) : IRequest<RequestResult<User>>;

    public class GetUserByIdHandler(IAuthRepository authRepository) : IRequestHandler<GetUserByIdQuery, RequestResult<User>>
    {
        private readonly IAuthRepository _authRepository = authRepository;

        public async Task<RequestResult<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User? user = await _authRepository.GetUserByIdAsync(request.userId);
            return user is null ? RequestResult<User>.Faild(ErrorCode.UserNotFound) : RequestResult<User>.Success(user);
        }
    }


}
