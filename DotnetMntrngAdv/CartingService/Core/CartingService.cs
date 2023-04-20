using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using Infrastructure.ServiceBus.DTO;
using Infrastructure.ServiceBus.Interfaces;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageListener _messageListener;

        public CartingService(ICartRepository cartRepository, IMessageListener messageListener)
        {
            _cartRepository = cartRepository;
            _messageListener = messageListener;
            _messageListener.Subscribe(ProcessItemChanges);
        }

        public async Task<Cart> GetCartInfo(string key)
        {
            return await Task.FromResult(_cartRepository.GetCart(key));
        }

        public async Task AddItem(string key, CartItem item)
        {
            await Task.Run(() => _cartRepository.AddItem(key, item));
        }

        public async Task RemoveItem(string key, int itemId)
        {
            await Task.Run(() => _cartRepository.Delete(key, itemId));
        }

        private void ProcessItemChanges(ItemDto dto)
        {
            var item = DtoToCartItem(dto);

            _cartRepository.UpdateItems(item);
        }

        private CartItem DtoToCartItem(ItemDto dto)
        {
            return new CartItem
            {
                Id = dto.Id,
                Name = dto.Name,
                Image = new Uri(dto.Image),
                Price = dto.Price,
                Quantity = dto.Quantity
            };
        }
    }
}