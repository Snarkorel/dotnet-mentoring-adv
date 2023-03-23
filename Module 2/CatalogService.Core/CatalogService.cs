using CatalogService.Core.Interfaces;
using CatalogService.Data.Interfaces;
using CatalogService.Data.Models;

namespace CatalogService.Core
{
    public class CatalogService : ICatalogService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CatalogService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public Category GetCategory(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Category> ListCategories()
        {
            throw new NotImplementedException();
        }

        public bool AddCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public Product GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Product> ListProducts()
        {
            throw new NotImplementedException();
        }

        public bool AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }
    }
}