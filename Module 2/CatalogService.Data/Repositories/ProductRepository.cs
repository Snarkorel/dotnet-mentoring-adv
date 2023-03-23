using CatalogService.Data.Interfaces;
using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(DbContext context) : base(context) { }
    }
}
