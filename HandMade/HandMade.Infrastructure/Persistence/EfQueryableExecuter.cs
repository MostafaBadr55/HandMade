using HandMade.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Persistence
{
    public class EfQueryableExecutor : IQueryableExecutor
    {
        public Task<int> CountAsync<T>(IQueryable<T> query, CancellationToken ct = default)
            => query.CountAsync(ct);

        public Task<List<T>> ToListAsync<T>(IQueryable<T> query, CancellationToken ct = default)
            => query.ToListAsync(ct);

        public async Task<T?> FirstOrDefaultAsync<T>(IQueryable<T> query, CancellationToken ct = default)
            => await query.FirstOrDefaultAsync(ct);
    }
}
