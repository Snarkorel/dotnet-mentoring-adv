using System.ComponentModel.DataAnnotations;

namespace CatalogService.Entities
{
    public class Product
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        public uint Amount { get; set; }
    }
}
