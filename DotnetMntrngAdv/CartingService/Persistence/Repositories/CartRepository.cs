using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using CartingService.Infrastructure.Repositories;

namespace CartingService.Persistence.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public Cart GetCart(string key)
        {
            var cart = _collection.Find(c => c.Key == key).First();
            if (cart == null)
                throw new ArgumentException($"Cart with key={key} not found!");

            return cart;
        }

        public void UpdateItems(CartItem item)
        {
            var carts = _collection.Find(c => c.Key != string.Empty);
            foreach (var cart in carts)
            {
                if (cart.Items.All(x => x.Id != item.Id))
                    continue;
                var existing = cart.Items.First(i => i.Id == item.Id);
                cart.Items.Remove(existing);
                cart.Items.Add(item);
                _collection.Update(cart);
            }
        }

        public void AddItem(string key, CartItem item)
        {
            var cart = _collection.Find(c => c.Key == key).ToList();
            if (!cart.Any())
            {
                var newCart = new Cart {Key = key, Items = new List<CartItem> {item}};
                _collection.Insert(newCart);
            }
            else
            {
                cart.First().Items.Add(item);
                _collection.Update(cart);
            }
                
        }

        public void Delete(string key, int itemId)
        {
            var cart = GetCart(key);
            var items = cart.Items;

            if (!items.Any())
                throw new ArgumentException($"Item with id={itemId} not found in cart with key={key}");
            
            var item = items.First(x => x.Id == itemId);
            cart.Items.Remove(item);
            _collection.Update(cart);
        }
    }
}
