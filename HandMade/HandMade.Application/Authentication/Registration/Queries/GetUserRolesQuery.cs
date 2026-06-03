using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Authentication.Registration.Queries
{
    public record GetUserRolesQuery(Guid userId) : IRequest<RequestResult<IList<string>>>;

    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, RequestResult<IList<string>>>
    {
        private readonly IAuthServices _authRepository;
        public GetUserRolesQueryHandler(IAuthServices authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<RequestResult<IList<string>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            IList<string> roles = await _authRepository.GetRolesAsync(request.userId);
            return roles == null || roles.Count == 0 ? RequestResult<IList<string>>.Faild(ErrorCode.NoRolesFound): RequestResult<IList<string>>.Success(roles);
        }
    }
}
