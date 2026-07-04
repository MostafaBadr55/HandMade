using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Features.Files.Commands.UploadImages.DTOs
{
    public record UploadImageResponseDTO(string RelativePath, string AbsoluteUrl);
}
