using CartingService.Core.Interfaces;
using CartingService.Infrastructure.Interfaces;
using CartingService.Infrastructure.LiteDB;
using CartingService.Core.Entities;
using CartingService.Infrastructure.Repositories;
using CatalogService.Core.Interfaces;
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
            //TestCartingService();
            TestCategoryService();
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

        private static void GetAndPrintItems(ICartingService service)
        {
            var items = service.GetItems().ToArray();
            PrintItems(items);
        }

        private static void TestCartingService()
        {
            Console.WriteLine("Testing Carting Service");

            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IDatabaseService<CartItem>, LiteDbDatabaseService<CartItem>>()
                .AddSingleton<ICartItemRepository, CartItemRepository>()
                .AddSingleton<ICartingService, CartingService.Core.CartingService>()
                .BuildServiceProvider();

            var cartingService = serviceProvider.GetService<ICartingService>();

            Console.WriteLine("Initializing CartingService...");
            cartingService.CreateCart(321);

            Console.WriteLine("Checking existing items in cart");
            GetAndPrintItems(cartingService);

            Console.WriteLine("Adding new item");
            cartingService.AddItem(new CartItem
                { Id = 1, Image = new Uri("https://google.com/logo.png"), Name = "testname", Price = 10.25m, Quantity = 1 });

            GetAndPrintItems(cartingService);

            Console.WriteLine("Adding another item");
            var item = new CartItem {Id = 2, Name = "Second test item", Price = 100.500m, Quantity = 15};
            cartingService.AddItem(item);

            GetAndPrintItems(cartingService);

            Console.WriteLine("Deleting items");
            cartingService.RemoveItem(1);
            cartingService.RemoveItem(2);

            GetAndPrintItems(cartingService);
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

        private static void TestCategoryService()
        {
            Console.WriteLine("Testing Category Service");

            var serviceProvider = new ServiceCollection()
                .AddDbContext<DbContext, CatalogContext>(ServiceLifetime.Transient)
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<ICatalogService, CatalogService.Core.CatalogService>()
                .BuildServiceProvider();

            var catalogService = serviceProvider.GetService<ICatalogService>();

            TestCategories(catalogService);
            TestProducts(catalogService);

            CleanupProducts(catalogService).Wait();
            CleanupCategories(catalogService).Wait();
        }
    }
}