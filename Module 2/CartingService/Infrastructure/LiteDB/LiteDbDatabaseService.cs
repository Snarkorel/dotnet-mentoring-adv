using CartingService.Infrastructure.Interfaces;
using LiteDB;
using System.Reflection;

namespace CartingService.Infrastructure.LiteDB
{
    internal class LiteDbDatabaseService<TEntity> : IDatabaseService<TEntity>
    {
        private readonly ILiteDatabase _database;
        //Let's imagine that we're taking this string from the config
        private static readonly string _connectionString = $"filename={Assembly.GetExecutingAssembly().Location}.LiteDb.db;journal=false";
        private readonly ILiteCollection<TEntity> _collection;

        internal LiteDbDatabaseService()
        {
            _database = new LiteDatabase(_connectionString);
            _collection = _database.GetCollection<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _collection.FindAll().ToList();
        }
        
        public void Add(TEntity entity)
        {
            _collection.Insert(entity);
        }

        public void Update(TEntity entity)
        {
            _collection.Update(entity);
        }

        public void Delete(int id)
        {
            _collection.Delete(id);
        }
    }
}
