using CartingService.Core.Entities;
using CartingService.Core.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        private Cart _cart;

        public CartingService(int cartId)
        {
            _cart = new Cart {Id = cartId};
        }
        
        public IEnumerable<CartItem> GetItems()
        {
            throw new NotImplementedException();
        }

        public bool AddItem(CartItem item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(int itemId)
        {
            throw new NotImplementedException();
        }
    }
}