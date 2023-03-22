namespace CartingService.Infrastructure.Interfaces
{
    public interface IDatabaseService<TEntity>
    {
        //Note: it's better to return IQueryable, but it's a LiteDB limitation, so IEnumerable used instead
        IEnumerable<TEntity> GetAll();

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Delete(int id);
    }
}
