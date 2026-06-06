using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IQueryableExecutor
    {
        Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default);
        Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct = default);
        Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct = default);
    }
}
