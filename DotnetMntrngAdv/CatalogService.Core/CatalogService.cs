using CatalogService.Domain.Entities;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Results;
using CatalogService.Core.Queries.Filters;
using Infrastructure.ServiceBus.Interfaces;
using Infrastructure.ServiceBus.DTO;
using Microsoft.Extensions.Logging;

namespace CatalogService.Core
{
    public class CatalogService : ICatalogService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<ICatalogService> _logger;

        public CatalogService(IProductRepository productRepository, ICategoryRepository categoryRepository, IMessagePublisher messagePublisher, ILogger<ICatalogService> logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        //TODO: some business logic should be there in almost each method

        public async Task<CategoryItem> GetCategory(int id)
        {
            _logger.LogInformation($"Getting category data for id={id}");
            var category = await _categoryRepository.GetCategoryAsync(id);
            return category;
        }
        
        public async Task<IEnumerable<CategoryItem>> ListCategories()
        {
            _logger.LogInformation($"Listing all categories");
            return await _categoryRepository.ListCategoriesAsync();
        }

        public async Task<int> AddCategory(CategoryItem category)
        {
            _logger.LogInformation($"Adding new category: [name = {category.Name}]");
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<bool> UpdateCategory(CategoryItem category)
        {
            _logger.LogInformation($"Modifying category [name = {category.Name}]");
            await _categoryRepository.UpdateCategoryAsync(category);
            _logger.LogInformation($"Successfully updated category [name = {category.Name}]");
            return true;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            _logger.LogInformation($"Deleting category [id = {id}]");
            await _categoryRepository.DeleteCategoryAsync(id);
            _logger.LogInformation($"Successfully deleted category [id = {id}]");
            return true;
        }

        public async Task<ProductItem> GetProduct(int id)
        {
            _logger.LogInformation($"Getting product info [id = {id}]");
            var product = await _productRepository.GetProductAsync(id);
            return product;
        }

        public async Task<IEnumerable<ProductItem>> ListProducts()
        {
            _logger.LogInformation($"Listing all products");
            return await _productRepository.ListProductsAsync();
        }

        public async Task<PagedResult<ProductItem>> ListProductsPaged(ProductFilter filter)
        {
            _logger.LogInformation($"Listing all products with filter [id = {filter.CategoryId}, page number = {filter.PageNumber}, page size = {filter.PageSize}]");
            return await _productRepository.ListProductsPaged(filter);
        }

        public async Task<int> AddProduct(ProductItem product)
        {
            _logger.LogInformation($"Adding new product [id = {product.Id}, name = {product.Name}, category = {product.Category.Name}, price = {product.Price}, amount = {product.Amount}]");
            return await _productRepository.AddProductAsync(product);
        }

        public async Task<bool> UpdateProduct(ProductItem product)
        {
            _logger.LogInformation($"Updating product [id = {product.Id}, name = {product.Name}, category = {product.Category.Name}]");
            await _productRepository.UpdateProductAsync(product);
            await NotifyOfProductChanges(product);
            return true;
        }

        public async Task<bool> DeleteProduct(int id)
        {
            _logger.LogInformation($"Deleting product [id = {id}]");
            await _productRepository.DeleteProductAsync(id);
            return true;
        }

        private async Task NotifyOfProductChanges(ProductItem item)
        {
            var dto = ProductToDto(item);
            _logger.LogInformation("Sending product update notification to message bus");
            await _messagePublisher.Send(dto);
        }

        private static ItemDto ProductToDto(ProductItem item)
        {
            return new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Image = item.Image ?? string.Empty,
                Price = item.Price,
                Quantity = (int)item.Amount
            };
        }
    }
}