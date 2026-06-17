using HandMade.Application.Interfaces;
using HandMade.Domain.Entities;
using HandMade.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HandMade.Infrastructure.Persistence
{
    public class GeneralRepository<T> : IGeneralRepository<T> where T: BaseModel
    {
        ApplicationDbContext _context;
        DbSet<T> _dbSet;
        public GeneralRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return GetAll().Where(expression);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.Where(x => !x.IsDeleted);
        }

        public IQueryable<T> GetRangeWithTracking(Expression<Func<T,bool>> expression)
        {
            return Get(expression).AsTracking();
        }

        public IQueryable<T> GetById(Guid id)
        {
            return _dbSet.Where(x => x.Id == id && !x.IsDeleted);
        }

        public IQueryable<T> GetByIdWithTracking(Guid id)
        {
            return _dbSet.Where(x => x.Id == id && !x.IsDeleted)
                .AsTracking();
        }

        public Task<bool> AnyAsync(Expression<Func<T,bool>> expression, CancellationToken ct)
        {
            return _dbSet.AnyAsync(expression,ct);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            entity.CreatedAt = DateTime.UtcNow;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
            foreach(var entity in entities){
                entity.CreatedAt = DateTime.UtcNow;
            }
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
             _dbSet.RemoveRange(entities);
        }
        public void SoftDelete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            entity.UpdatedAt = DateTime.UtcNow;
        }

        public Task<int> SaveChanges()
        => _context.SaveChangesAsync();
    }
}
