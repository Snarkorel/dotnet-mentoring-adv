using CatalogService.Domain.Entities;

namespace CatalogService.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<ProductItem> GetProductAsync(int id);

        Task<IEnumerable<ProductItem>> ListProductsAsync();

        Task AddProductAsync(ProductItem item);

        Task UpdateProductAsync(ProductItem item);

        Task DeleteProductAsync(int id);
    }
}
