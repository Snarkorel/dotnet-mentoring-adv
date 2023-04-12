using CartingService.Core.Entities;
using CartingService.Core.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        private readonly ICartRepository _cartRepository;

        public CartingService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart> GetCartInfo(string key)
        {
            return await Task.FromResult(_cartRepository.GetCart(key));
        }

        public async Task AddItem(string key, CartItem item)
        {
            await Task.Run(() => _cartRepository.AddItem(key, item));
        }

        public async Task RemoveItem(string key, int itemId)
        {
            await Task.Run(() => _cartRepository.Delete(key, itemId));
        }
        
    }
}