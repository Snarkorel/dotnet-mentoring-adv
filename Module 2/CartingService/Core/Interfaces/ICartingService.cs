using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartingService
    {
        void Initialize(int cartId);

        IEnumerable<CartItem> GetItems();

        bool AddItem(CartItem item);

        bool Remove(int itemId);
    }
}
