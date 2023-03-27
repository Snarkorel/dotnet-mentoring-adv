using CatalogService.Domain.Entities;
using CatalogService.Data.Models;

namespace CatalogService.Core.Helpers
{
    public static class ProductHelper
    {
        //public static IQueryable<ProductItem> ProductsToProductItems(this IQueryable<Product> products)
        //{
        //    return products.Select(x => new ProductItem
        //    {
        //        Name = x.Name,
        //        Description = string.IsNullOrEmpty(x.Description) ? null : x.Description,
        //        Image = x.Image == null ? null : new Uri(x.Image),
        //        Category = CategoryHelper.CategoryToCategoryItem(x.Category),
        //        Price = x.Price,
        //        Amount = x.Amount
        //    });
        //}

        //public static ProductItem ProductToProductItem(Product product)
        //{
        //    return new ProductItem
        //    {
        //        Name = product.Name,
        //        Description = string.IsNullOrEmpty(product.Description) ? null : product.Description,
        //        Image = product.Image == null ? null : new Uri(product.Image),
        //        Category = CategoryHelper.CategoryToCategoryItem(product.Category),
        //        Price = product.Price,
        //        Amount = product.Amount
        //    };
        //}

        //public static Product ProductItemToProduct(ProductItem item)
        //{
        //    return new Product
        //    {
        //        Name = item.Name,
        //        Description = string.IsNullOrEmpty(item.Description) ? null : item.Description,
        //        Image = item.Image == null ? null : item.Image.ToString(),
        //        Category = CategoryHelper.CategoryItemToCategory(item.Category),
        //        Price = item.Price,
        //        Amount = item.Amount
        //    };
        //}
    }
}
