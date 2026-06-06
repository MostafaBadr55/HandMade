using HandMade.Application.Interfaces;
using HandMade.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Helpers
{
    public static class PaginationHelper
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query, IQueryableExecutor executor, int pageNumber, int pageSize,      CancellationToken cancelationToken = default)
        {
            if (pageNumber < 1) pageNumber = DefaultPageNumber;
            if (pageSize < 1) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            int totalCount = await executor.CountAsync(query, cancelationToken);

            List<T> items = await executor.ToListAsync(
                query.Skip((pageNumber - 1) * pageSize).Take(pageSize), cancelationToken);

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
