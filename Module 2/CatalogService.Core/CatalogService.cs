using CatalogService.Core.Entities;
using CatalogService.Core.Helpers;
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

        //some business logic should be there

        public CategoryItem GetCategory(int id)
        {
            var item = _categoryRepository.Get(id);
            var category = new CategoryItem {Name = item.Name};

            if (!string.IsNullOrEmpty(item.Image))
                category.Image = new Uri(item.Image);

            if (item.ParentCategoryId != null)
            {
                category.ParentCategory = GetCategory(item.ParentCategoryId.Value);
                category.ParentCategoryId = item.ParentCategoryId;
            }
            
            return category;
        }
        
        public IQueryable<CategoryItem> ListCategories()
        {
            return _categoryRepository.List().CategoriesToCategoryItems();
        }

        public bool AddCategory(CategoryItem category)
        {
            _categoryRepository.Add(new Category
            {
                Name = category.Name,
                Image = category.Image != null ? category.Image.ToString() : string.Empty,
                ParentCategoryId = category.ParentCategoryId
            });

            return true;
        }

        public bool UpdateCategory(CategoryItem category)
        {
            _categoryRepository.Update(CategoryHelper.CategoryItemToCategory(category));
            return true;
        }

        public bool DeleteCategory(int id)
        {
            _categoryRepository.Delete(id);

            return true;
        }

        public ProductItem GetProduct(int id)
        {
            var product = _productRepository.Get(id);
            return ProductHelper.ProductToProductItem(product);
        }

        public IQueryable<ProductItem> ListProducts()
        {
            return _productRepository.List().ProductsToProductItems();
        }

        public bool AddProduct(ProductItem product)
        {
            _productRepository.Add(ProductHelper.ProductItemToProduct(product));
            return true;
        }

        public bool UpdateProduct(ProductItem product)
        {
            _productRepository.Update(ProductHelper.ProductItemToProduct(product));
            return true;
        }

        public bool DeleteProduct(int id)
        {
            _productRepository.Delete(id);
            return true;
        }
    }
}