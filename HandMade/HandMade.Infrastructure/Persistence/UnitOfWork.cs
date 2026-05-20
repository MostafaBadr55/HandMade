using HandMade.Application.Interfaces;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Infrastructure.Persistence
{
    /// <summary>
    /// Implements IUnitOfWork using EF Core.
    /// Scoped lifetime — one instance per HTTP request, matching DbContext lifetime.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = [];

        public UnitOfWork(ApplicationDbContext dbContext)
            => _dbContext = dbContext;

        /// <inheritdoc/>
        public IGeneralRepository<TEntity> GetRepository<TEntity>()
            where TEntity : BaseModel
        {
            var entityType = typeof(TEntity);

            if (_repositories.TryGetValue(entityType, out var existing))
                return (IGeneralRepository<TEntity>)existing;

            var repo = new GeneralRepository<TEntity>(_dbContext);
            _repositories[entityType] = repo;
            return repo;
        }

        /// <inheritdoc/>
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            var tx = await _dbContext.Database.BeginTransactionAsync(ct);
            return new EfTransaction(tx);
        }

        /// <inheritdoc/>
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _dbContext.SaveChangesAsync(ct);
    }
}
