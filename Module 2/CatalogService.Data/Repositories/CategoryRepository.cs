using CatalogService.Core.Interfaces;
using CatalogService.Data.Models;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(DbContext context) : base(context) { }

        public CategoryItem Get(int id)
        {
            var category = base.Get(id);
            
            throw new NotImplementedException();
        }

        public async Task<CategoryItem> GetAsync(int id)
        {
            var category = await base.GetAsync(id);

            throw new NotImplementedException();
        }

        public IEnumerable<CategoryItem> List()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CategoryItem>> ListAsync()
        {
            throw new NotImplementedException();
        }

        public void Add(CategoryItem item)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(CategoryItem item)
        {
            throw new NotImplementedException();
        }

        public void Update(CategoryItem item)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(CategoryItem item)
        {
            throw new NotImplementedException();
        }
    }
}
