using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Data.Models
{
    public class Product : Entity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        
        public Category Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        public uint Amount { get; set; }
    }
}
