using CatalogService.Core.Entities;
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

        public CategoryItem GetCategory(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<CategoryItem> ListCategories()
        {
            throw new NotImplementedException();
        }

        public bool AddCategory(CategoryItem category)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCategory(CategoryItem category)
        {
            throw new NotImplementedException();
        }

        public bool DeleteCategory(int id)
        {
            throw new NotImplementedException();
        }

        public ProductItem GetProduct(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProductItem> ListProducts()
        {
            throw new NotImplementedException();
        }

        public bool AddProduct(ProductItem product)
        {
            throw new NotImplementedException();
        }

        public bool UpdateProduct(ProductItem product)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProduct(int id)
        {
            throw new NotImplementedException();
        }
    }
}