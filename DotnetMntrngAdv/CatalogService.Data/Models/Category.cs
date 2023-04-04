using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Data.Models
{
    public class Category : DbEntity
    {
        [Required]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.ImageUrl)]
        public string? Image { get; set; }
        
        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
