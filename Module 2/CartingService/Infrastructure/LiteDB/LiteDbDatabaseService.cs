using LiteDB;

namespace CartingService.Infrastructure.LiteDB
{
    internal class LiteDBDatabaseService<TEntity> : IDatabaseService<TEntity>
    {
        private ILiteDatabase _database;
        private readonly ILiteCollection<TEntity> _collection;

        internal LiteDBDatabaseService(string connectionString)
        {
            //TODO: initialize db, check if db file exists, otherwise - create it
            _database = new LiteDatabase(connectionString);

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
