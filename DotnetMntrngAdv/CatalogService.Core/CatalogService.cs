using CatalogService.Domain.Entities;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Results;
using CatalogService.Core.Queries.Filters;

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

        //TODO: some business logic should be there in almost each method

        public async Task<CategoryItem> GetCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryAsync(id);
            return category;
        }
        
        public async Task<IEnumerable<CategoryItem>> ListCategories()
        {
            return await _categoryRepository.ListCategoriesAsync();
        }

        public async Task<int> AddCategory(CategoryItem category)
        {
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<bool> UpdateCategory(CategoryItem category)
        {
            await _categoryRepository.UpdateCategoryAsync(category);
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
            return true;
        }

        public async Task<ProductItem> GetProduct(int id)
        {
            var product = await _productRepository.GetProductAsync(id);
            return product;
        }

        public async Task<IEnumerable<ProductItem>> ListProducts()
        {
            return await _productRepository.ListProductsAsync();
        }

        public async Task<PagedResult<ProductItem>> ListProductsPaged(ProductFilter filter)
        {
            return await _productRepository.ListProductsPaged(filter);
        }

        public async Task<int> AddProduct(ProductItem product)
        {
            return await _productRepository.AddProductAsync(product);
        }

        public async Task<bool> UpdateProduct(ProductItem product)
        {
            await _productRepository.UpdateProductAsync(product);
            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return true;
        }
    }
}