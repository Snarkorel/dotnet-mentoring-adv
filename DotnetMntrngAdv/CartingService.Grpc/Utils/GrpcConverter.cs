using CartingService.Core.Entities;
using Google.Protobuf.Collections;
using System.Globalization;

namespace CartingService.Grpc.Utils
{
    internal static class GrpcConverter
    {
        internal static decimal ToDecimal(this string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        internal static RepeatedField<Item> ConvertItems(IEnumerable<CartItem> items)
        {
            var convertedItems = new RepeatedField<Item>();
            foreach (var item in items)
            {
                convertedItems.Add(item.ToItem());
            }

            return convertedItems;
        }

        internal static Item ToItem(this CartItem item)
        {
            return new Item
            {
                Id = item.Id,
                Image = item.Image.ToString(),
                Name = item.Name,
                Price = item.Price.ToString(CultureInfo.InvariantCulture),
                Quantity = item.Quantity,
            };
        }

        internal static CartItem ToCartItem(this Item item)
        {
            return new CartItem
            {
                Id = item.Id,
                Image = new Uri(item.Image),
                Name = item.Name,
                Price = item.Price.ToDecimal(),
                Quantity = item.Quantity
            };
        }
    }
}
