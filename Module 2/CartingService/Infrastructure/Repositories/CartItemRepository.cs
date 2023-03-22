using CartingService.Core.Entities;
using CartingService.Infrastructure.Interfaces;

namespace CartingService.Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IDatabaseService<CartItem> _dbService;
        
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
            _dbService.Add(item);
        }

        public void Update(CartItem item)
        {
            //constraints are not in the scope of task
            _dbService.Update(item);
        }

        public void Delete(int id)
        {
            _dbService.Delete(id);
        }
    }
}
