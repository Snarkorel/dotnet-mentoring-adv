using CartingService.Core.Entities;
using CatalogService.Domain.Entities;

namespace Infrastructure.ServiceBus.DTO
{
    public class ItemDto
    {
        int Id { get; set; }
        string Name { get; set; }
        string Image { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }

        public static ItemDto ToDto(ProductItem item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Image = item.Image ?? string.Empty,
                Price = item.Price,
                Quantity = (int)item.Amount
            };
        }

        public CartItem ToCartItem()
        {
            return new CartItem
            {
                Id = Id,
                Name = Name,
                Image = new Uri(Image),
                Price = Price,
                Quantity = Quantity
            };
        }
    }
}
