using CatalogService.Core.Interfaces;
using CatalogService.Data.Models;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context) { }

        public ProductItem Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductItem> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductItem> List()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductItem>> ListAsync()
        {
            throw new NotSupportedException();
        }

        public void Add(ProductItem item)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(ProductItem item)
        {
            throw new NotImplementedException();
        }

        public void Update(ProductItem item)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ProductItem item)
        {
            throw new NotImplementedException();
        }


    }
}
