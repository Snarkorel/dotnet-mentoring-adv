using CartingService.Core.Entities;

namespace CartingService.Infrastructure.Interfaces
{
    public interface ICartItemRepository : IRepository
    {
        IEnumerable<CartItem> GetItems();

        void Add(CartItem item);

        void Update(CartItem item);

        void Delete(int id);
    }
}
