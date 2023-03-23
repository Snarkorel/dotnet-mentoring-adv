using CatalogService.Data.Interfaces;
using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public abstract class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly DbContext _context;
        
        protected GenericRepository(DbContext context)
        {
            _context = context;
        }
        
        public TEntity Get(int id)
        {
            return _context.Find<TEntity>(id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public IQueryable<TEntity> List()
        {
            return _context.Set<TEntity>();
        }

        public void Add(TEntity item)
        {
            _context.Add(item);
        }

        public void Update(TEntity item)
        {
            _context.Update(item);
        }

        public void Delete(TEntity item)
        {
            _context.Remove(item);
        }
    }
}
