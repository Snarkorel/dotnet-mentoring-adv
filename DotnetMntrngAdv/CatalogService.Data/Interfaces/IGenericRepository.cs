using System.Linq.Expressions;

namespace CatalogService.Core.Interfaces
{
    public interface IGenericRepository<TEntity>
    {
        Task<TEntity> GetAsync(int id);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> ListAsync();

        Task AddAsync(TEntity item);

        Task UpdateAsync(TEntity item);

        Task DeleteAsync(int id);
    }
}
