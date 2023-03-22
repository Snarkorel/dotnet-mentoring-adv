namespace CartingService.Infrastructure.Interfaces
{
    internal interface IDatabaseService<TEntity>
    {
        IEnumerable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(int id);
    }
}
