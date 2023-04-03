using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Data.Models
{
    public class Product : DbEntity
    {
        [Required]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Html)]
        public string? Description { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? Image { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        
        public Category Category { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public uint Amount { get; set; }
    }
}
