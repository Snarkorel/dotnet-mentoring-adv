using CatalogService.Data.Interfaces;
using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DbContext context) : base(context) { }
    }
}
