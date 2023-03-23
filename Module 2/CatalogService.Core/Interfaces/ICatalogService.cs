using CatalogService.Core.Entities;

namespace CatalogService.Core.Interfaces
{
    public interface ICatalogService
    {
        CategoryItem GetCategory(int id);

        IQueryable<CategoryItem> ListCategories();

        bool AddCategory(CategoryItem category);

        bool UpdateCategory(CategoryItem category);

        bool DeleteCategory(int id);

        ProductItem GetProduct(int id);

        IQueryable<ProductItem> ListProducts();

        bool AddProduct(ProductItem product);

        bool UpdateProduct(ProductItem product);

        bool DeleteProduct(int id);
    }
}
