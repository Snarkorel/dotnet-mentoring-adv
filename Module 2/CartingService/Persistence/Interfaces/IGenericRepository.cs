namespace CartingService.Infrastructure.Interfaces
{
    public interface IGenericRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetAll();

        Task Add(TEntity entity);

        Task Update(TEntity entity);

        Task Delete(int id);
    }
}
