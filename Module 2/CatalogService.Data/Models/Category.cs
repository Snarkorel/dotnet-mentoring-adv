using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService.Data.Models
{
    public class Category : Entity
    {
        [Required]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }

        public Category ParentCategory { get; set; }
    }
}
