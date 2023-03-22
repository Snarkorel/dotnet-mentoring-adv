using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartingService
    {
        IEnumerable<CartItem> GetItems();

        bool AddItem(CartItem item);

        bool Remove(int itemId);
    }
}
