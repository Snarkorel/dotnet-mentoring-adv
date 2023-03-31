namespace CatalogService.Core.Interfaces
{
    public interface IGenericRepository<TEntity>
    {
        //TEntity Get(int id);

        Task<TEntity> GetAsync(int id);

        //IEnumerable<TEntity> List();

        Task<IEnumerable<TEntity>> ListAsync();

        //void Add(TEntity item);

        Task AddAsync(TEntity item);

        //void Update(TEntity item);

        Task UpdateAsync(TEntity item);

        //void Delete(int id);

        Task DeleteAsync(int id);
    }
}
