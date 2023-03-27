using CatalogService.Core.Interfaces;
using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public abstract class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : DbEntity
    {
        private readonly DbContext _context;
        
        //Null checks, logging and exception processing skipped here because it's out of the task scope
        protected GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public virtual TEntity Get(int id)
        {
            return _context.Find<TEntity>(id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public async Task<TEntity> GetAsync(int id)
        {
            return await _context.FindAsync<TEntity>(id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public virtual IEnumerable<TEntity> List()
        {
            return _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> ListAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public virtual void Add(TEntity item)
        {
            _context.Add(item);
            _context.SaveChanges();
        }

        public virtual async Task AddAsync(TEntity item)
        {
            await _context.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public virtual void Update(TEntity item)
        {
            _context.Update(item);
            _context.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity item)
        {
            _context.Update(item);
            await _context.SaveChangesAsync();
        }
        
        public virtual void Delete(int id)
        {
            var entity = Get(id);
            _context.Remove(entity);
            _context.SaveChanges();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = GetAsync(id);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
