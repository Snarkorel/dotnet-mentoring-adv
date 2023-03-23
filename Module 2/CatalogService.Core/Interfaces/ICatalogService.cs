using CatalogService.Data.Models;

namespace CatalogService.Core.Interfaces
{
    public interface ICatalogService
    {
        Category GetCategory(int id);

        IQueryable<Category> ListCategories();

        bool AddCategory(Category category);

        bool UpdateCategory(Category category);

        bool DeleteCategory(int id);

        Product GetProduct(int id);

        IQueryable<Product> ListProducts();

        bool AddProduct(Product product);

        bool UpdateProduct(Product product);

        bool DeleteProduct(int id);
    }
}
