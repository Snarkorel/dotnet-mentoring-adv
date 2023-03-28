namespace CatalogService.Domain.Entities
{
    public class CategoryItem : Item
    {
        public string Name { get; set; }

        public string? Image { get; set; }

        public CategoryItem? ParentCategory { get; set; }
    }
}
