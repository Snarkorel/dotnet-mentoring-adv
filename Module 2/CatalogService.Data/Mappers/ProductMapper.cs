using CatalogService.Data.Models;
using CatalogService.Domain.Entities;

namespace CatalogService.Data.Mappers
{
    public static class ProductMapper
    {
        public static ProductItem ToProductItem(this Product product)
        {
            return new ProductItem
            {
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Category = product.Category.ToCategoryItem(),
                Price = product.Price,
                Amount = product.Amount
            };
        }

        public static Product ToProduct(this ProductItem item)
        {
            return new Product
            {
                Name = item.Name,
                Description = item.Description,
                Image = item.Image,
                Category = item.Category.ToCategory(),
                Price = item.Price,
                Amount = item.Amount
            };
        }
    }
}
