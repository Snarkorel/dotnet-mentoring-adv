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
            TestCartingService();
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

            Console.WriteLine("Deleting item");
            cartingService.RemoveItem(1);

            GetAndPrintItems(cartingService);
            
            //first item is intentionally not deleted and persists in database
        }

        private static void PrintCategory(CategoryItem item)
        {
            Console.WriteLine(
                $"[Category]\r\nName = {item.Name}\r\nImage = {item.Image}\r\nParent Category Id = {item.ParentCategory?.Id}\r\nParent Category = {item.ParentCategory}");
        }

        private static void PrintCategories(IEnumerable<CategoryItem> items)
        {
            Console.WriteLine("\r\nCategories:");
            foreach (var item in items)
            {
                PrintCategory(item);
            }
        }

        private static void GetAndPrintCategories(ICatalogService service)
        {
            var categories = service.ListCategories();
            PrintCategories(categories.Result);
        }

        private static void PrintProduct(ProductItem item)
        {
            Console.WriteLine(
                $"[Product]\r\nName = {item.Name}\r\nDescription = {item.Description}\r\nImage = {item.Image}\r\nCategory = {item.Category}\r\nPrice = {item.Price}\r\nAmount = {item.Amount}");
        }

        private static void PrintProducts(IEnumerable<ProductItem> items)
        {
            Console.WriteLine("\r\nItems:");
            foreach (var item in items)
            {
                PrintProduct(item);
            }
        }

        private static void GetAndPrintProducts(ICatalogService service)
        {
            var products = service.ListProducts();
            PrintProducts(products.Result);
        }

        private static void TestCategoryService()
        {
            Console.WriteLine("Testing Category Service");

            var serviceProvider = new ServiceCollection()
                .AddSingleton<DbContext, CatalogContext>()
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<ICatalogService, CatalogService.Core.CatalogService>()
                .BuildServiceProvider();

            var catalogService = serviceProvider.GetService<ICatalogService>();

            Console.WriteLine("Categories methods");

            Console.WriteLine("Listing categories");
            GetAndPrintCategories(catalogService);

            Console.WriteLine("Adding category");
            var category = new CategoryItem {Name = "Main category", Image = "http://localhost/img.png", ParentCategory = null};
            catalogService.AddCategory(category);
            GetAndPrintCategories(catalogService);

            var id = 5; //todo: dynamic id update based on returned value

            Console.WriteLine("Modifying category");
            category = catalogService.GetCategory(id).Result;
            category.Image = "http://localhost/image.png";
            catalogService.UpdateCategory(category);
            GetAndPrintCategories(catalogService);

            Console.WriteLine("Adding nested category");
            var nestedCategory = new CategoryItem
            {
                Name = "Child category",
                Image = "http://127.0.0.1/logo.png",
                ParentCategory = category
            };
            catalogService.AddCategory(nestedCategory);
            GetAndPrintCategories(catalogService);

            //Delete tests turned off
            //Console.WriteLine($"Deleting category with id = {id+1}");
            //catalogService.DeleteCategory(id+1);
            //GetAndPrintCategories(catalogService);
            //Console.WriteLine($"Deleting category with id = {id}");
            //catalogService.DeleteCategory(id);
            //GetAndPrintCategories(catalogService);

            Console.WriteLine("Categories methods test completed");

            Console.WriteLine("Products methods");

            Console.WriteLine("Listing products");
            GetAndPrintProducts(catalogService);

            Console.WriteLine("Adding product");
            var product = new ProductItem { Name = "Soap", Description = "Regular hygienic soap", Image = "http://localhost/soap.png", Category = category, Amount = 2, Price = 12.5m };
            catalogService.AddProduct(product);
            GetAndPrintProducts(catalogService);

            id = 1; //todo: dynamic unique id from db

            Console.WriteLine("Modifying product");
            product = catalogService.GetProduct(id).Result;
            product.Price = 16.25m;
            catalogService.UpdateProduct(product);
            GetAndPrintProducts(catalogService);

            //Console.WriteLine($"Deleting product with id = {id}");
            //catalogService.DeleteProduct(id);
            //GetAndPrintProducts(catalogService);

            Console.WriteLine("Products methods test completed");
        }
    }
}