namespace CatalogService.Core.Queries.Filters
{
    public class ProductFilter : PagedQuery
    {
        public int? CategoryId { get; set; }
    }
}
