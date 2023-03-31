using CartingService.Core.Entities;
using CartingService.Core.Interfaces;

namespace CartingService.Infrastructure.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public async Task<IEnumerable<CartItem>> GetItems()
        {
            return await GetAll();
        }
    }
}
