using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using CartingService.Infrastructure.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        private Cart _cart;
        private readonly ICartItemRepository _repository;
        
        //TODO: logging, exception processing (out of task's scope)
        private CartingService(ICartItemRepository repository)
        {
            _repository = repository;
        }

        public CartingService(int cartId)
        {
            _cart = new Cart {Id = cartId};
        }
        
        public IEnumerable<CartItem> GetItems()
        {
            return _repository.GetItems();
        }

        public bool AddItem(CartItem item)
        {
            if (ItemExists(item.Id))
                IncreaseCountIfExists(item);
            else
                _repository.Add(item);

            return true;
        }

        public bool Remove(int itemId)
        {
            if (!ItemExists(itemId))
                return false;

            _repository.Delete(itemId);
            return true;
        }

        private bool ItemExists(int id)
        {
            return GetItems().Any(x => x.Id == id);
        }

        private void IncreaseCountIfExists(CartItem item)
        {
            var existingItem = GetItems().First(x => x.Id == item.Id);
            existingItem.Quantity += item.Quantity;
            _repository.Update(existingItem);
        }

        
    }
}