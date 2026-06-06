using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HandMade.Domain.Specifications
{
    public abstract class BaseSpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; protected set; } = _ => true;
        public Expression<Func<T, object>>? OrderBy { get; protected set; }
        public Expression<Func<T, object>>? OrderByDescending { get; protected set; }
    }
}
