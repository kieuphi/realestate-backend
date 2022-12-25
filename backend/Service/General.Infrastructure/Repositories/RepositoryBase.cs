using Microsoft.EntityFrameworkCore;
using General.Application.Common.Interfaces;
using General.Domain.Common;
using General.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Shared.Entities;
using Common.Shared.Enums;
using System.Reflection;

namespace General.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : AuditableEntity
    {
        protected readonly ApplicationDbContext _dbContext;

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }
        public IQueryable<T> WhereIgnoreDelete(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            query = query.Where(t => t.IsDeleted == DeletedStatus.False);
            return query;
        }
        public Expression<Func<T, bool>> CreateExpressionCompareField(string field,object searchText)
        {
            var paramData = Expression.Parameter(typeof(T));
            var propertyData = Expression.PropertyOrField(paramData, field);
            var valueData = Expression.Constant(searchText);

            var compareExpression = System.Linq.Expressions.Expression.Equal(propertyData, valueData);
            
            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(compareExpression, paramData);
            return predicate;
        }
        public Expression<Func<T, bool>> CreateExpressionContainsData(string field, string searchText)
        {
            var paramData = Expression.Parameter(typeof(T));
            var propertyData = Expression.PropertyOrField(paramData, field);
            var valueData = Expression.Constant(searchText);

            MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var expressionContains = Expression.Call(propertyData, containsMethod, valueData);
            

            Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(expressionContains, paramData);
            return predicate;
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public void Add(T entity)
        {
            entity.IsDeleted = DeletedStatus.False;
            _dbContext.Set<T>().Add(entity);
        }
        public async Task<T> AddAsync(T entity)
        {
            entity.IsDeleted = DeletedStatus.False;
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = DeletedStatus.True;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
