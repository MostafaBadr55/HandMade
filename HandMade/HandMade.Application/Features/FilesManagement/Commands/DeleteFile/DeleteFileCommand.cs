using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Files.Commands.DeleteFile
{
    public record DeleteFileCommand(string RelativePath) : IRequest<RequestResult<bool>>;

    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, RequestResult<bool>>
    {
        private readonly IStorageService _storageService;

        public DeleteFileCommandHandler(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<RequestResult<bool>> Handle(
            DeleteFileCommand request,
            CancellationToken cancellationToken)
        {
            var deleted = await _storageService.DeleteAsync(request.RelativePath);

            if (!deleted)
                return RequestResult<bool>.Failed(ErrorCode.FileNotFound);

            return RequestResult<bool>.Success(true);
        }
    }
}
