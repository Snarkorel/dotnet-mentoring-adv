using CatalogService.Core.Queries.Filters;
using CatalogService.Core.Queries.Results;
using CatalogService.Domain.Entities;

namespace CatalogService.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductItem> GetProductAsync(int id);

        Task<IEnumerable<ProductItem>> ListProductsAsync();

        Task<PagedResult<ProductItem>> ListProductsPaged(ProductFilter filter);

        Task<int> AddProductAsync(ProductItem item);

        Task UpdateProductAsync(ProductItem item);

        Task DeleteProductAsync(int id);
    }
}
