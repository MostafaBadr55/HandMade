using HandMade.Application.Features.Files.Commands.UploadImages;
using HandMade.Helpers;
using HandMade.ViewModels.UploadFiles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HandMade.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Seller")]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FileUploadResponseVM>> Upload(
            [FromForm] FileUploadRequestVM request,
            [FromQuery] UploadTarget target = UploadTarget.Product,
            CancellationToken cancellationToken = default)
        {
            var file = request.File;

            var command = new UploadImageCommand(
                file.OpenReadStream(),
                file.FileName,
                file.Length,
                target);

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return result.ErrorCode.ToProblem(HttpContext.Request.Path);

            var response = new FileUploadResponseVM { AbsoluteUrl = result.Data.AbsoluteUrl, RelativePath = result.Data.RelativePath };
            return response;
        }
    }
}
