using CatalogService.Data.Models;

namespace CatalogService.Data.Interfaces
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        TEntity Get(int id);

        IEnumerable<TEntity> List();

        void Add(TEntity item);

        void Update(TEntity item);

        void Delete(int id);
    }
}
