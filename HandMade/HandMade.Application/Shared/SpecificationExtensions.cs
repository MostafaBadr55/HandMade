using HandMade.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Shared
{
    public static class SpecificationExtensions
    {
        public static IQueryable<T> ApplySpecification<T>(
            this IQueryable<T> query,
            BaseSpecification<T> spec)
        {
            query = query.Where(spec.Criteria);

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);

            return query;
        }
    }
}
