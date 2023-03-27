using CatalogService.Domain.Entities;
using CatalogService.Core.Interfaces;

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

        public async Task <CategoryItem> GetCategory(int id)
        {

            //var category = new CategoryItem {Name = item.Name};

            //if (!string.IsNullOrEmpty(item.Image))
            //    category.Image = new Uri(item.Image);

            //if (item.ParentCategoryId != null)
            //{
            //    category.ParentCategory = GetCategory(item.ParentCategoryId.Value);
            //    category.ParentCategoryId = item.ParentCategoryId;
            //}

            var category = await _categoryRepository.GetAsync(id);
            return category;
        }
        
        public async Task <IEnumerable<CategoryItem>> ListCategories()
        {
            return await _categoryRepository.ListAsync();
        }

        public async Task<bool> AddCategory(CategoryItem category)
        {
            //_categoryRepository.Add(new Category
            //{
            //    Name = category.Name,
            //    Image = category.Image != null ? category.Image.ToString() : string.Empty,
            //    ParentCategoryId = category.ParentCategoryId
            //});

            await _categoryRepository.AddAsync(category);
            return true;
        }

        public async Task<bool> UpdateCategory(CategoryItem category)
        {
            //_categoryRepository.Update(CategoryHelper.CategoryItemToCategory(category));

            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<ProductItem> GetProduct(int id)
        {
            //return ProductHelper.ProductToProductItem(product);

            var product = await _productRepository.GetAsync(id);
            return product;
        }

        public async Task <IEnumerable<ProductItem>> ListProducts()
        {
            return await _productRepository.ListAsync();//.ProductsToProductItems();
        }

        public async Task <bool> AddProduct(ProductItem product)
        {
            //_productRepository.Add(ProductHelper.ProductItemToProduct(product));

            await _productRepository.AddAsync(product);
            return true;
        }

        public async Task <bool> UpdateProduct(ProductItem product)
        {
            //_productRepository.Update(ProductHelper.ProductItemToProduct(product));
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            await _productRepository.DeleteAsync(id);
            return true;
        }
    }
}