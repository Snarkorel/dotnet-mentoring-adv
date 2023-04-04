using CatalogService.Core.Queries.Filters;
using CatalogService.Core.Queries.Results;
using CatalogService.Domain.Entities;

namespace CatalogService.Core.Interfaces
{
    public interface ICatalogService
    {
        Task<CategoryItem> GetCategory(int id);

        Task<IEnumerable<CategoryItem>> ListCategories();
        
        Task<bool> AddCategory(CategoryItem category);

        Task<bool> UpdateCategory(CategoryItem category);

        Task<bool> DeleteCategory(int id);

        Task<ProductItem> GetProduct(int id);

        Task<IEnumerable<ProductItem>> ListProducts();

        Task<PagedResult<ProductItem>> ListProductsPaged(ProductFilter filter);

        Task<bool> AddProduct(ProductItem product);

        Task<bool> UpdateProduct(ProductItem product);

        Task<bool> DeleteProduct(int id);
    }
}
