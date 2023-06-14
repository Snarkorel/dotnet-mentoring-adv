using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using Infrastructure.ServiceBus.DTO;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;

namespace CartingService.Core
{
    public class CartingService : ICartingService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMessageListener _messageListener;
        private readonly ILogger<ICartingService> _logger;

        public CartingService(ICartRepository cartRepository, IMessageListener messageListener, ILogger<ICartingService> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _messageListener = messageListener;
            _messageListener.Subscribe(ProcessItemChanges);
        }

        public async Task<Cart> GetCartInfo(string key)
        {
            _logger.LogInformation($"Getting cart info for cart '{key}'");
            return await Task.FromResult(_cartRepository.GetCart(key));
        }

        public async Task AddItem(string key, CartItem item)
        {
            _logger.LogInformation($"Adding a new item [id={item.Id} name={item.Name} price={item.Price} quantity={item.Quantity}] to cart '{key}'");
            await Task.Run(() => _cartRepository.AddItem(key, item));
        }

        public async Task RemoveItem(string key, int itemId)
        {
            _logger.LogInformation($"Removing item id={itemId} from cart '{key}'");
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