using CartingService.Core.Interfaces;
using CartingService.Infrastructure.Interfaces;
using CartingService.Infrastructure.LiteDB;
using CartingService.Core.Entities;
using CartingService.Infrastructure.Repositories;
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
            cartingService.Initialize(321);

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
            cartingService.Remove(1);

            GetAndPrintItems(cartingService);
            
            //first item is intentionally not deleted and persists in database
        }

        private static void TestCategoryService()
        {
            Console.WriteLine("Testing Category Service");

            //TODO!
        }
    }
}