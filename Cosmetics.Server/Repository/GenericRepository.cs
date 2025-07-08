using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CMS.Server.Models;
using CMS.Server.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace CMS.Server.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AMSDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AMSDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveChangesAsync();
            await Task.CompletedTask; // Required for consistency with async APIs
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask; // Required for consistency with async APIs
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction(); // EF Core transaction
        }

        public IQueryable<T> GetDbSet()
        {
            return _context.Set<T>();
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate)
                => await _dbSet.FirstOrDefaultAsync(predicate);

        public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);
    }

    //public async Task<T> GetByNameAsync(string name, Expression<Func<T, string>> nameSelector)
    //{
    //    if (string.IsNullOrWhiteSpace(name))
    //        throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

    //    return await _dbSet.FirstOrDefaultAsync(e => nameSelector.Compile()(e) == name);
    //}


}
