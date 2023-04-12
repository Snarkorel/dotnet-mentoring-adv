using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartingService
    {
        Task<Cart> GetCartInfo(string key);

        Task AddItem(string key, CartItem item);

        Task RemoveItem(string key, int itemId);
    }
}
