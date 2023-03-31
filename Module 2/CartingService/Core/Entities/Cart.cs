using System.ComponentModel.DataAnnotations;

namespace CartingService.Core.Entities
{
    public class Cart
    {
        [Required]
        public int Id { get; init; }

        public List<CartItem> Items { get; }
    }
}
