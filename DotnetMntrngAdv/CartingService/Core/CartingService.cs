using CartingService.Core.Entities;
using CartingService.Core.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        //TODO: cart repository with only get/add methods
        private readonly ICartItemRepository _repository;

        //TODO: logging, exception processing
        public CartingService(ICartItemRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateCart(int cartId)
        {
            //TODO: add method as required in module 3
            throw new NotImplementedException();
        }
        
        public async Task<IEnumerable<CartItem>> GetItems()
        {
            return await _repository.GetItems();
        }

        public async Task<bool> AddItem(CartItem item)
        {
            await _repository.Add(item);
            return true;
        }

        public async Task<bool> RemoveItem(int itemId)
        {
            await _repository.Delete(itemId);
            return true;
        }
        
    }
}