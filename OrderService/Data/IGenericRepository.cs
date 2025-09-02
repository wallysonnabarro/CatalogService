using System.Linq.Expressions;

namespace OrderService.Data
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        Task<T> ByExpressionData(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindPagedAsync(Expression<Func<T, bool>> predicate, int skip, int take, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T> FirstWithIncludsAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task SaveChangesAsync();
        void SaveChanges();
        Task<int> CountAsync(Expression<Func<T, bool>> expression,
            Func<IQueryable<T>, IQueryable<T>>? include = null);
    }
}
