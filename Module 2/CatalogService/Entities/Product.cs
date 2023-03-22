using System.ComponentModel.DataAnnotations;

namespace CatalogService.Core.Entities
{
    public class Product
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Image { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        public uint Amount { get; set; }
    }
}
