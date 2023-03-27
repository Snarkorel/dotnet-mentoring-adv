using System.ComponentModel.DataAnnotations;

namespace CatalogService.Domain.Entities
{
    public class CategoryItem
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public CategoryItem ParentCategory { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}
