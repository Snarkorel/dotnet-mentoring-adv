namespace CatalogService.Domain.Entities
{
    public class ProductItem : Item
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public CategoryItem Category { get; set; }

        public decimal Price { get; set; }

        public uint Amount { get; set; }
    }
}
