using HandMade.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HandMade.Application.Interfaces
{
    public interface IGeneralRepository<T> where T: BaseModel
    {
        // Read
        public IQueryable<T> GetAll();
        public IQueryable<T> GetById(Guid id);
        public IQueryable<T> Get(Expression<Func<T, bool>> expression);
        public IQueryable<T> GetByIdWithTracking(Guid id);

        // Write
        public void Add(T entity);
        public void Update(T entity);
        public bool? SoftDeleteById(Guid id);
        public void AddRange(IEnumerable<T> entities);

        public Task<int> SaveChanges();
    }
}
