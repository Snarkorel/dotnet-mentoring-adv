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
        
        public virtual TEntity Get(int id)
        {
            return _context.Find<TEntity>(id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public virtual IQueryable<TEntity> List()
        {
            return _context.Set<TEntity>();
        }

        public virtual void Add(TEntity item)
        {
            _context.Add(item);
        }

        public virtual void Update(TEntity item)
        {
            _context.Update(item);
        }

        public virtual void Delete(TEntity item)
        {
            _context.Remove(item);
        }
    }
}
