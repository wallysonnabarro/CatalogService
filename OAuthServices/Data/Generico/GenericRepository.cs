using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace OAuthServices.Data.Generico
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ContextDb _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ContextDb context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Entidade do tipo {typeof(T).Name} com ID {id} não foi encontrada.");
            }

            return entity;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> IColletionAsync(Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.Where(expression).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindPagedAsync(
        Expression<Func<T, bool>> predicate,
        int skip,
        int take,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query
                .Where(predicate)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public virtual async Task<T> FirstWithIncludsAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query
                .FirstAsync(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public virtual async Task<T> ByExpressionData(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstAsync(expression);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public virtual void SaveChanges()
        {
            _context.SaveChanges();
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.Where(expression).CountAsync();
        }
    }
}
