using CartingService.Core.Interfaces;
using CartingService.Core.Entities;
using CartingService.Infrastructure.Repositories;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Filters;
using CatalogService.Data.Database;
using CatalogService.Data.Repositories;
using CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TestApp initialized");
            TestCartingService();
            TestCatalogService();
        }

        private static void PrintCartItem(CartItem item)
        {
            Console.WriteLine($"[CartItem]\r\nId = {item.Id}\r\nName = {item.Name}\r\nImage URL = {item.Image}\r\nPrice = {item.Price}\r\nQuantity = {item.Quantity}\r\n");
        }

        private static void PrintItems(IEnumerable<CartItem> items)
        {
            Console.WriteLine("\r\nItems in cart:");
            foreach (var item in items)
            {
                PrintCartItem(item);
            }
        }

        private static async Task DeleteItems(ICartingService service)
        {
            Console.WriteLine("Removing items...");
            var items = await service.GetItems();
            foreach (var item in items)
            {
                Console.WriteLine($"Removing item with id={item.Id}");
                await service.RemoveItem(item.Id);
            }
        }

        private static async Task GetAndPrintItems(ICartingService service)
        {
            var items = await service.GetItems();
            PrintItems(items);
        }

        private static void TestCartingService()
        {
            Console.WriteLine("Testing Carting Service");

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ICartItemRepository, CartItemRepository>()
                .AddSingleton<ICartingService, CartingService.Core.CartingService>()
                .BuildServiceProvider();

            var cartingService = serviceProvider.GetService<ICartingService>();

            Console.WriteLine("Initializing CartingService...");
            //cartingService.CreateCart(321); //TODO: use later in module 3

            Console.WriteLine("Checking existing items in cart");
            GetAndPrintItems(cartingService).Wait();

            Console.WriteLine("Adding new item");
            cartingService.AddItem(new CartItem
            {
                Image = new Uri("https://google.com/logo.png"), 
                Name = "testname", 
                Price = 10.25m, 
                Quantity = 1
            });

            GetAndPrintItems(cartingService).Wait();

            Console.WriteLine("Adding another item");
            var item = new CartItem 
            {
                Name = "Second test item", 
                Price = 100.500m, 
                Quantity = 15

            };
            cartingService.AddItem(item);

            GetAndPrintItems(cartingService).Wait();
            DeleteItems(cartingService).Wait();
            GetAndPrintItems(cartingService).Wait();
        }

        private static void PrintCategory(CategoryItem item)
        {
            Console.WriteLine(
                $"[Category]\r\nId = {item.Id}\r\nName = {item.Name}\r\nImage = {item.Image}\r\nParent Category = {item.ParentCategory?.Id} | {item.ParentCategory?.Name}");
        }

        private static void PrintCategories(IEnumerable<CategoryItem> items)
        {
            Console.WriteLine("\r\nCategories:");
            foreach (var item in items)
            {
                PrintCategory(item);
            }
        }

        private static async Task GetAndPrintCategories(ICatalogService service)
        {
            var categories = await service.ListCategories();
            PrintCategories(categories);
        }

        private static async Task CleanupCategories(ICatalogService service)
        {
            Console.WriteLine("Cleaning up categories...");
            var categories = await service.ListCategories();
            foreach (var category in categories)
            {
                Console.WriteLine($"Cleaning up category with id={category.Id}");
                await service.DeleteCategory(category.Id);
            }
        }

        private static void PrintProduct(ProductItem item)
        {
            Console.WriteLine(
                $"[Product]\r\nId = {item.Id}\r\nName = {item.Name}\r\nDescription = {item.Description}\r\nImage = {item.Image}\r\nCategory = [{item.Category.Id}] {item.Category.Name}\r\nPrice = {item.Price}\r\nAmount = {item.Amount}");
        }

        private static void PrintProducts(IEnumerable<ProductItem> items)
        {
            Console.WriteLine("\r\nItems:");
            foreach (var item in items)
            {
                PrintProduct(item);
            }
        }

        private static async Task GetAndPrintProducts(ICatalogService service)
        {
            var products = await service.ListProducts();
            PrintProducts(products);
        }

        private static async Task CleanupProducts(ICatalogService service)
        {
            Console.WriteLine("Cleaning up products...");
            var products = await service.ListProducts();
            foreach (var product in products)
            {
                Console.WriteLine($"Cleaning up product with id={product.Id}");
                await service.DeleteProduct(product.Id);
            }
        }

        private static void TestCategories(ICatalogService catalogService)
        {
            Console.WriteLine("Categories methods");

            Console.WriteLine("Listing categories");
            GetAndPrintCategories(catalogService).Wait();
            
            Console.WriteLine("Adding category");
            var category = new CategoryItem
            {
                Name = "Main category", 
                Image = "http://localhost/img.png", 
                ParentCategory = null
            };
            catalogService.AddCategory(category).Wait();
            GetAndPrintCategories(catalogService).Wait();

            Console.WriteLine("Modifying category");
            var id = catalogService.ListCategories().Result.First().Id;
            category = catalogService.GetCategory(id).Result;
            category.Image = "http://localhost/image.png";
            catalogService.UpdateCategory(category).Wait();
            GetAndPrintCategories(catalogService).Wait();

            Console.WriteLine("Adding nested category");
            var parentCategory = new CategoryItem
            {
                Name = "Parent category"
            };
            catalogService.AddCategory(parentCategory).Wait();

            parentCategory = catalogService.ListCategories().Result.Last();
            var nestedCategory = new CategoryItem
            {
                Name = "Child category",
                Image = "http://127.0.0.1/logo.png",
                ParentCategory = parentCategory
            };
            catalogService.AddCategory(nestedCategory).Wait();
            GetAndPrintCategories(catalogService).Wait();
            
            Console.WriteLine("Categories get/modify methods test completed");
        }

        private static void TestProducts(ICatalogService catalogService)
        {
            Console.WriteLine("Products methods");

            Console.WriteLine("Listing products");
            GetAndPrintProducts(catalogService).Wait();
            
            Console.WriteLine("Adding product");
            var category = catalogService.ListCategories().Result.Last();
            var product = new ProductItem
            {
                Name = "Soap", 
                Description = "Regular hygienic soap", 
                Image = "http://localhost/soap.png", 
                Category = category, 
                Amount = 2,
                Price = 12.5m
            };
            catalogService.AddProduct(product).Wait();
            GetAndPrintProducts(catalogService).Wait();
            
            Console.WriteLine("Modifying product");
            var id = catalogService.ListProducts().Result.First().Id;
            product = catalogService.GetProduct(id).Result;
            product.Price = 16.25m;
            catalogService.UpdateProduct(product).Wait();
            GetAndPrintProducts(catalogService).Wait();

            Console.WriteLine("Products get/modify methods test completed");
        }

        private static void TestProductsRemovalWithCategory(ICatalogService catalogService)
        {
            Console.WriteLine("Testing Products removal on Category deletion");

            Console.WriteLine("Listing products");
            GetAndPrintProducts(catalogService).Wait();

            Console.WriteLine("Creating category");
            var newCategory = new CategoryItem
            {
                Name = "Some category"
            };
            catalogService.AddCategory(newCategory).Wait();
            var category = catalogService.ListCategories().Result.First();
            

            Console.WriteLine("Creating and adding products");
            var firstItem = new ProductItem
            {
                Name = "First item",
                Price = 100m,
                Amount = 1,
                Category = category
            };

            var secondItem = new ProductItem
            {
                Name = "Second Item",
                Price = 0.01m,
                Amount = 2,
                Category = category
            };
            catalogService.AddProduct(firstItem).Wait();
            catalogService.AddProduct(secondItem).Wait();

            Console.WriteLine("Performing ListProductsPaged tests");
            Console.WriteLine("Pagination test: Page number = 1, Page size = 1");
            var filter = new ProductFilter {PageNumber = 1, PageSize = 1};
            var products = catalogService.ListProductsPaged(filter).Result;
            PrintProducts(products);

            Console.WriteLine("Pagination test: Page number = 2, Page size = 1");
            filter.PageNumber = 2;
            products = catalogService.ListProductsPaged(filter).Result;
            PrintProducts(products);

            var firstProductId = catalogService.ListProducts().Result.Select(x => x.Id).First();
            Console.WriteLine($"Pagination test: Page number = 1, Page size = 10, Id (filter) = {firstProductId}");
            filter.ProductId = firstProductId;
            filter.PageNumber = 1;
            filter.PageSize = 10;
            products = catalogService.ListProductsPaged(filter).Result;
            PrintProducts(products);

            GetAndPrintCategories(catalogService).Wait();
            GetAndPrintProducts(catalogService).Wait();

            Console.WriteLine("Removing all categories");
            CleanupCategories(catalogService).Wait();

            Console.WriteLine("Checking, if we still have products");
            GetAndPrintProducts(catalogService).Wait();
        }

        private static void CatalogServiceCleanup(ICatalogService catalogService)
        {
            CleanupProducts(catalogService).Wait();
            CleanupCategories(catalogService).Wait();
        }

        private static void TestCatalogService()
        {
            Console.WriteLine("Testing Catalog Service");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<DbContext, CatalogContext>(ServiceLifetime.Transient)
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<ICatalogService, CatalogService.Core.CatalogService>()
                .BuildServiceProvider();

            var catalogService = serviceProvider.GetService<ICatalogService>();

            TestCategories(catalogService);
            TestProducts(catalogService);

            CatalogServiceCleanup(catalogService);

            TestProductsRemovalWithCategory(catalogService);

            CatalogServiceCleanup(catalogService);
        }
    }
}