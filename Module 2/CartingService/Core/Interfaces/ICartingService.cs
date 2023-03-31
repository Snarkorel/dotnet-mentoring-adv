using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartingService
    {
        Task CreateCart(int cartId);

        Task<IEnumerable<CartItem>> GetItems();

        Task<bool> AddItem(CartItem item);

        Task<bool> RemoveItem(int itemId);
    }
}
