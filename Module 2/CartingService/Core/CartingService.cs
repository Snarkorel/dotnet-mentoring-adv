using CartingService.Core.Entities;
using CartingService.Core.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
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