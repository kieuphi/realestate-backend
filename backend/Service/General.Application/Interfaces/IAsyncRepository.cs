using Common.Shared.Entities;
using General.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace General.Application.Common.Interfaces
{
    public interface IAsyncRepository<T> where T : AuditableEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        string includeString = null,
                                        bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                       List<Expression<Func<T, object>>> includes = null,
                                       bool disableTracking = true);
        IQueryable<T> WhereIgnoreDelete(Expression<Func<T, bool>> predicate);
        Expression<Func<T, bool>> CreateExpressionCompareField(string field, object searchText);
        Expression<Func<T, bool>> CreateExpressionContainsData(string field, string searchText);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        void Add(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> SaveChangesAsync();
    }
}
