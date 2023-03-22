using CartingService.Core.Entities;
using CartingService.Infrastructure.Interfaces;

namespace CartingService.Infrastructure.Repositories
{
    internal class CartItemRepository : ICartItemRepository
    {
        private readonly IDatabaseService<CartItem> _dbService;

        //TODO: DI
        public CartItemRepository(IDatabaseService<CartItem> dbService)
        {
            _dbService = dbService;
        }
        
        public IEnumerable<CartItem> GetItems()
        {
            return _dbService.GetAll();
        }

        public void Add(CartItem item)
        {
            //TODO: constraints
            _dbService.Add(item);
        }

        public void Update(CartItem item)
        {
            //TODO: constraints
            _dbService.Update(item);
        }

        public void Delete(int id)
        {
            _dbService.Delete(id);
        }
    }
}
