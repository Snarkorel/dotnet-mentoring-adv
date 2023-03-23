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

        public CategoryItem GetCategory(int id)
        {
            var item = _categoryRepository.Get(id);
            var category = new CategoryItem {Name = item.Name};

            if (!string.IsNullOrEmpty(item.Image))
                category.Image = new Uri(item.Image);

            if (item.ParentCategoryId != null)
                category.ParentCategory = GetCategory(item.ParentCategoryId.Value);

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
                Image = category.Image.ToString(),
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