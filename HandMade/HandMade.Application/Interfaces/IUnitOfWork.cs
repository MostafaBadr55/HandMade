using HandMade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Interfaces
{
    /// <summary>
    /// Coordinates multiple repository operations under a single DbContext.
    /// Caller is responsible for the transaction lifetime (Option A).
    /// Register as Scoped in DI — one instance per HTTP request.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Returns a cached or newly created repository for TEntity.
        /// All repositories share the same DbContext, so changes
        /// across multiple repos are tracked together.
        /// </summary>
        IGeneralRepository<TEntity> GetRepository<TEntity>()
            where TEntity : BaseModel;

        /// <summary>
        /// Begins a DB-level transaction and returns it to the caller.
        /// Caller is responsible for CommitAsync / RollbackAsync / DisposeAsync.
        ///
        /// Usage:
        ///   await using var tx = await _uow.BeginTransactionAsync();
        ///   try   { ... await _uow.SaveChangesAsync(); await tx.CommitAsync(); }
        ///   catch { await tx.RollbackAsync(); throw; }
        /// </summary>
        Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Flushes all EF Core tracked changes to the database
        /// in a single round-trip. Call once after all mutations.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

}
