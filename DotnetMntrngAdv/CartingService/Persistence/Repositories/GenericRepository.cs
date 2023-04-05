using CartingService.Infrastructure.Interfaces;
using LiteDB;
using System.Reflection;

namespace CartingService.Infrastructure.Repositories
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity>
    {
        //Actually, LiteDB doesn't support asynchronous approach, so it faked here
        //Let's imagine that we're taking this string from the config
        private static readonly string _connectionString = $"filename={Assembly.GetExecutingAssembly().Location}.LiteDb.db;journal=false";
        protected readonly ILiteCollection<TEntity> _collection;

        protected GenericRepository()
        {
            ILiteDatabase database = new LiteDatabase(_connectionString);
            _collection = database.GetCollection<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Task.FromResult(_collection.FindAll().ToList());
        }

        public async Task Add(TEntity entity)
        {
            await Task.FromResult(_collection.Insert(entity));
        }

        public async Task Update(TEntity entity)
        {
            await Task.FromResult(_collection.Update(entity));
        }

        public async Task Delete(int id)
        {
            await Task.FromResult(_collection.Delete(id));
        }
    }
}
