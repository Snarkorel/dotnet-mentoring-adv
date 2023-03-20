using CartingService.Entities;

namespace CartingService.Interfaces
{
    public interface ICartingService
    {
        IEnumerable<CartItem> GetItems();

        bool AddItem(CartItem item);

        bool Remove(int itemId);
    }
}
