namespace CatalogService.Core.Queries.Filters
{
    public class ProductFilter : PagedQuery
    {
        public int? ProductId { get; set; }
    }
}
