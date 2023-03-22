using System.ComponentModel.DataAnnotations;

namespace CartingService.Core.Entities
{
    public class CartItem
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Image { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}
