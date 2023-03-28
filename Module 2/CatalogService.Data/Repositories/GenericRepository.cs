using CatalogService.Core.Interfaces;
using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CatalogService.Data.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : DbEntity
    {
        private readonly DbContext _context;
        
        //Null checks, logging and exception processing skipped here because it's out of the task scope
        protected GenericRepository(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public TEntity Get(int id)
        {
            return _context.Set<TEntity>().FirstOrDefault(x => x.Id == id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public async Task<TEntity> GetAsync(int id)
        {
            return await _context.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
        }

        public IEnumerable<TEntity> List()
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public async Task<IEnumerable<TEntity>> ListAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public void Add(TEntity item)
        {
            _context.Set<TEntity>().AddAsync(item);
            _context.SaveChanges();
        }

        public async Task AddAsync(TEntity item)
        {
            await _context.Set<TEntity>().AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public void Update(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _context.Set<TEntity>().Update(item);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            //workaround for mapping - detach old tracked entitiy and attach a new one
            if (_context.Entry(item).State == EntityState.Detached)
            {
                var attachedEntity = _context.Set<TEntity>()
                    .Local
                    .FirstOrDefault(entry => entry.Id.Equals(item.Id));
                if (attachedEntity != null)
                {
                    // detach
                    _context.Entry(attachedEntity).State = EntityState.Detached;
                }
            }
            _context.Entry(item).State = EntityState.Modified;
            
            //_context.Set<TEntity>().Update(item);
            await _context.SaveChangesAsync();
        }
        
        public void Delete(int id)
        {
            var entity = Get(id);
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
