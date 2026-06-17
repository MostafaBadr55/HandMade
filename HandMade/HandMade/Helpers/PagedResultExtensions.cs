using HandMade.Application.Shared;
using HandMade.ViewModels;

namespace HandMade.Helpers
{
    public static class PagedResultExtensions
    {
        public static PagedResponseVM<TResponse> ToPagedResponseVM<TDto, TResponse>(
            this PagedResult<TDto> pagedResult,Func<TDto, TResponse> itemMapper)
        {
            return new PagedResponseVM<TResponse>
            {
                Items = pagedResult.Items.Select(itemMapper).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasNextPage = pagedResult.HasNextPage,
                HasPreviousPage = pagedResult.HasPreviousPage
            };
        }
    }
}
