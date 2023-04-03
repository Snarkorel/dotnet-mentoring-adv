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
            var product = await _context.Set<Product>().Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id) ?? throw new InvalidOperationException($"Object with id={id} not found in DB");
            return product.ToProductItem();
        }

        public async Task<IEnumerable<ProductItem>> ListProductsAsync()
        {
            var products = await _context.Set<Product>().Include(p => p.Category).AsNoTracking().ToListAsync();
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
