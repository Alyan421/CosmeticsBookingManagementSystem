using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace CMS.Server.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        public IQueryable<T> GetDbSet();
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    }
}
