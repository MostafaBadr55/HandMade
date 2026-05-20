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

        public IQueryable<T> GetById(Guid id)
        {
            return _dbSet.Where(x => x.Id == id && !x.IsDeleted);
        }

        public IQueryable<T> GetByIdWithTracking(Guid id)
        {
            return _dbSet.Where(x => x.Id == id && !x.IsDeleted)
                .AsTracking();
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


        public bool? SoftDeleteById(Guid id)
        {
            T? entity = GetByIdWithTracking(id).FirstOrDefault();
            if (entity is null)
                return null;

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;

            return true;


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
