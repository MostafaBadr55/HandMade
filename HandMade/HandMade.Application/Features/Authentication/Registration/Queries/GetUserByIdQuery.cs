using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.CQRS.Authentication.Registration.Queries
{
    public record GetUserByIdQuery(Guid userId) : IRequest<RequestResult<User>>;

    public class GetUserByIdHandler(IAccountServices authRepository) : IRequestHandler<GetUserByIdQuery, RequestResult<User>>
    {
        private readonly IAccountServices _authRepository = authRepository;

        public async Task<RequestResult<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            User? user = await _authRepository.GetUserByIdAsync(request.userId);
            return user is null ? RequestResult<User>.Failed(ErrorCode.UserNotFound) : RequestResult<User>.Success(user);
        }
    }


}
