using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartRepository
    {
        Cart GetCart(string key);

        void UpdateItems(CartItem item);

        void AddItem(string key, CartItem item);

        void Delete(string key, int itemId);
    }
}
