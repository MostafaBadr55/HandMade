using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using HandMade.Domain.Entities;
using MediatR;

namespace HandMade.Application.Features.Orders.Commands.CreateOrderAttachments
{
    /// <summary>
    /// Stores the reference images the client uploaded with a request. Paths are
    /// root-relative — IUrlBuilder makes them absolute at read time.
    /// </summary>
    public record CreateOrderAttachmentsCommand(
        Guid OrderId,
        IReadOnlyList<string> RelativePaths) : IRequest<RequestResult<bool>>;

    public class CreateOrderAttachmentsCommandHandler(IUnitOfWork _unitOfWork)
        : IRequestHandler<CreateOrderAttachmentsCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(
            CreateOrderAttachmentsCommand request,
            CancellationToken cancellationToken)
        {
            if (request.RelativePaths.Count == 0)
                return RequestResult<bool>.Success(true);

            var attachments = request.RelativePaths
                .Select((path, index) => new OrderAttachment
                {
                    OrderId = request.OrderId,
                    Url = path,
                    SortOrder = index
                })
                .ToList();

            _unitOfWork.GetRepository<OrderAttachment>().AddRange(attachments);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResult<bool>.Success(true);
        }
    }
}
