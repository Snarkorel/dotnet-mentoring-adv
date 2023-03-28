using CatalogService.Core.Interfaces;
using CatalogService.Data.Mappers;
using CatalogService.Data.Models;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context) { }
        
        public async Task<ProductItem> GetProductAsync(int id)
        {
            var product = await GetAsync(id);
            return product.ToProductItem();
        }

        public async Task<IEnumerable<ProductItem>> ListProductsAsync()
        {
            var products = await ListAsync();
            return products.Select(x => x.ToProductItem());
        }

        public async Task AddProductAsync(ProductItem item)
        {
            await AddAsync(item.ToProduct());
        }

        public async Task UpdateProductAsync(ProductItem item)
        {
            await UpdateAsync(item.ToProduct());
        }

        public async Task DeleteProductAsync(int id)
        {
            await DeleteAsync(id);
        }
    }
}
