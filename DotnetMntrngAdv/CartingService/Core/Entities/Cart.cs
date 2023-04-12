using LiteDB;
using System.ComponentModel.DataAnnotations;

namespace CartingService.Core.Entities
{
    public class Cart
    {
        [Required]
        [BsonId]
        public string Key { get; init; }

        public List<CartItem> Items { get; init; }
    }
}
