using HandMade.Application.Features.Files.Commands.UploadImages.DTOs;
using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Files.Commands.UploadImages
{
    public record UploadImageCommand(Stream FileStream,string FileName,long FileSizeInBytes,        UploadTarget Target): IRequest<RequestResult<UploadImageResponseDTO>>;

    public class UploadImageCommandHandler
    : IRequestHandler<UploadImageCommand, RequestResult<UploadImageResponseDTO>>
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IStorageService _storage;
        private readonly IUrlBuilder _urlBuilder;

        public UploadImageCommandHandler(IStorageService storage, IUrlBuilder urlBuilder)
        {
            _storage = storage;
            _urlBuilder = urlBuilder;
        }

        public async Task<RequestResult<UploadImageResponseDTO>> Handle(
            UploadImageCommand request,
            CancellationToken cancellationToken)
        {
            if (request.FileSizeInBytes == 0)
                return RequestResult<UploadImageResponseDTO>.Failed(ErrorCode.NoFileProvided);

            if (request.FileSizeInBytes > MaxFileSizeInBytes)
                return RequestResult<UploadImageResponseDTO>.Failed(ErrorCode.FileTooLarge);

            var ext = Path.GetExtension(request.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(ext))
                return RequestResult<UploadImageResponseDTO>.Failed(ErrorCode.InvalidFileExtension);

            var folder = ResolveFolder(request.Target);
            var newFileName = $"{Guid.NewGuid()}{ext}";

            var relativePath = await _storage.SaveFileAsync(
                request.FileStream,
                newFileName,
                folder,
                cancellationToken);

            var absoluteUrl = _urlBuilder.BuildAbsoluteUrl(relativePath);

            return RequestResult<UploadImageResponseDTO>.Success(
                new UploadImageResponseDTO(relativePath, absoluteUrl));
        }

        private static string ResolveFolder(UploadTarget target) => target switch
        {
            UploadTarget.Shop => "uploads/shops",
            UploadTarget.Product => "uploads/products",
            _ => "uploads/general"
        };
    }
}
