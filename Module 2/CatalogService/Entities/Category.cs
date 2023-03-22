using System.ComponentModel.DataAnnotations;

namespace CatalogService.Core.Entities
{
    public class Category
    {
        [Required]
        public string Name { get; set; }

        public string Image { get; set; }

        public Category ParentCategory { get; set; }
    }
}
