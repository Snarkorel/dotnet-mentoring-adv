using System.ComponentModel.DataAnnotations;

namespace CatalogService.Core.Entities
{
    public class CategoryItem
    {
        [Required]
        public string Name { get; set; }

        public Uri Image { get; set; }

        public CategoryItem ParentCategory { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
