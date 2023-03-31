using CartingService.Core.Entities;

namespace CartingService.Core.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetItems();

        Task Add(CartItem item);

        Task Update(CartItem item);

        Task Delete(int id);
    }
}
