using CartingService.Core.Interfaces;
using CartingService.Core.Entities;
using CartingService.Persistence.Repositories;
using CatalogService.Core.Interfaces;
using CatalogService.Core.Queries.Filters;
using CatalogService.Data.Database;
using CatalogService.Data.Repositories;
using CatalogService.Domain.Entities;
using Infrastructure.ServiceBus;
using Infrastructure.ServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace TestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TestApp initialized");
            //TestCartingService();
            //TestCatalogService();
            //TestMessaging();
            TestCatalogServiceAuthorization();
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

        private static async Task DeleteItems(ICartingService service, string cartKey)
        {
            Console.WriteLine("Removing items...");
            var cart = await service.GetCartInfo(cartKey);
            foreach (var item in cart.Items)
            {
                Console.WriteLine($"Removing item with id={item.Id}");
                await service.RemoveItem(cartKey, item.Id);
            }
        }

        private static async Task GetAndPrintItems(ICartingService service, string cartKey)
        {
            var cart = await service.GetCartInfo(cartKey);
            Console.WriteLine($"Cart contents [key={cart.Key}]:");
            PrintItems(cart.Items);
        }

        private static ICartingService GetCartingService()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<ICartRepository, CartRepository>()
                .AddSingleton<IMessageListener, MessageListener>()
                .AddSingleton<ICartingService, CartingService.Core.CartingService>()
                .BuildServiceProvider();

            var cartingService = serviceProvider.GetService<ICartingService>();

            return cartingService;
        }

        private static void TestCartingService()
        {
            Console.WriteLine("Testing Carting Service");

            var cartingService = GetCartingService();

            var cartKey = "cart123";
            var firstItem = new CartItem
            {
                Id = 1,
                Image = new Uri("https://google.com/logo.png"),
                Name = "testname",
                Price = 10.25m,
                Quantity = 1
            };
            Console.WriteLine($"Initializing cart [{cartKey} with one item]");
            cartingService.AddItem(cartKey, firstItem).Wait();
            GetAndPrintItems(cartingService, cartKey).Wait();

            Console.WriteLine("Adding another item");
            var secondItem = new CartItem 
            {
                Id = 2,
                Name = "Second test item", 
                Price = 100.500m, 
                Quantity = 15

            };
            cartingService.AddItem(cartKey, secondItem).Wait();
            GetAndPrintItems(cartingService, cartKey).Wait();

            Console.WriteLine("Removing first item");
            firstItem = cartingService.GetCartInfo(cartKey).Result.Items.First();
            cartingService.RemoveItem(cartKey, firstItem.Id).Wait();
            GetAndPrintItems(cartingService, cartKey).Wait();

            Console.WriteLine("Removing second item");
            secondItem = cartingService.GetCartInfo(cartKey).Result.Items.Last();
            cartingService.RemoveItem(cartKey, secondItem.Id).Wait();
            GetAndPrintItems(cartingService, cartKey).Wait();
        }

        private static async Task CartingServiceCleanup(ICartingService cartingService, string cartKey)
        {
            var items = cartingService.GetCartInfo(cartKey).Result.Items;
            foreach (var item in items)
            {
                cartingService.RemoveItem(cartKey, item.Id).Wait();
            }
            GetAndPrintItems(cartingService, cartKey).Wait();
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
            filter.CategoryId = category.Id;
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

        private static ICatalogService GetCatalogService()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<DbContext, CatalogContext>(ServiceLifetime.Transient)
                .AddSingleton<ICategoryRepository, CategoryRepository>()
                .AddSingleton<IProductRepository, ProductRepository>()
                .AddSingleton<IMessagePublisher, MessagePublisher>()
                .AddSingleton<ICatalogService, CatalogService.Core.CatalogService>()
                .BuildServiceProvider();

            var catalogService = serviceProvider.GetService<ICatalogService>();

            return catalogService;
        }

        private static void TestCatalogService()
        {
            Console.WriteLine("Testing Catalog Service");

            var catalogService = GetCatalogService();

            TestCategories(catalogService);
            TestProducts(catalogService);

            CatalogServiceCleanup(catalogService);

            TestProductsRemovalWithCategory(catalogService);

            CatalogServiceCleanup(catalogService);
        }

        private static void TestMessaging()
        {
            Console.WriteLine("Testing messaging");

            var catalogService = GetCatalogService();
            var cartingService = GetCartingService();

            var cartName = "TestCart";
            var category = new CategoryItem
            {
                Name = "Base category"
            };
            catalogService.AddCategory(category).Wait();
            var actualCategory = catalogService.ListCategories().Result.First();

            var product = new ProductItem
            {
                Name = "ServiceBusTest",
                Image = "http://localhost/img.jpg",
                Price = 10.20m,
                Amount = 2,
                Category = actualCategory
            };
            catalogService.AddProduct(product).Wait();
            var actualProduct = catalogService.ListProducts().Result.First();

            var cartItem = new CartItem
            {
                Id = actualProduct.Id,
                Image = new Uri(actualProduct.Image),
                Name = actualProduct.Name,
                Price = actualProduct.Price,
                Quantity = (int)actualProduct.Amount
            };
            cartingService.AddItem(cartName, cartItem).Wait();

            actualProduct.Name = "Modified product test for ServiceBus";
            catalogService.UpdateProduct(actualProduct).Wait();

            Thread.Sleep(60000);

            GetAndPrintItems(cartingService, cartName).Wait();

            CatalogServiceCleanup(catalogService);
            CartingServiceCleanup(cartingService, cartName).Wait();
        }

        private static void TestBuyerAuthorization()
        {
            Console.WriteLine("Testing Buyer access");
            
            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_BUYER_CLIENT_ID";
            //Client secret, obviously, shouldn't be hardcoded like this and should be retrieved securely, but for learning task it's OK
            var clientSecret = "CHANGE_ME_BUYER_CLIENT_SECRET";
            
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            if (authResult != null)
            {
                Console.WriteLine("Access token: " + authResult.AccessToken);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var successUrl = "https://localhost:7021/categories";
                var failUrl = "https://localhost:7021/category";

                Console.WriteLine($"Calling {successUrl}");
                string categoriesJson = client.GetStringAsync(successUrl).Result;
                Console.WriteLine($"Server response:\r\n{categoriesJson}");

                //This should fail - we don't have manager permissions
                var category = "{\r\n\t\"name\": \"post category\",\r\n\t\"image\": \"http://localhost/img.png\",\r\n\t\"parentCategory\": null\r\n}";
                var content = new StringContent(category, Encoding.UTF8, "application/json");
                var responseCode = client.PostAsync(failUrl, content).Result.StatusCode;
                if (responseCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine(
                        "Got 403 Forbidden response code while trying to do Manager-only action having Buyer permissions");
                }
                else
                {
                    Console.WriteLine($"Unexpected response code: {responseCode}");
                }
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static void TestManagerAuthorization()
        {
            Console.WriteLine("Testing Manager access");

            //Azure AD tenant ID
            var tenantId = "<CHANGE_ME_TENANT>";
            //WebAPI app registration client ID
            var apiAppClientId = "CHANGE_ME_API_CLIENT_ID";
            var scopes = new[] { $"api://{apiAppClientId}/.default" };
            var authority = $"https://login.microsoftonline.com/{tenantId}";

            //Client app registration client ID
            var clientId = "CHANGE_ME_MANAGER_CLIENT_ID";
            var clientSecret = "CHANGE_ME_MANAGER_CLIENT_SECRET";

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithClientSecret(clientSecret)
                .Build();
            var authResult = app.AcquireTokenForClient(scopes).ExecuteAsync().Result;

            if (authResult != null)
            {
                Console.WriteLine("Access token: " + authResult.AccessToken);

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                var addCategoryUrl = "https://localhost:7021/category";
                var getCategoriesUrl = "https://localhost:7021/categories";

                var category = "{\r\n\t\"name\": \"post category\",\r\n\t\"image\": \"http://localhost/img.png\",\r\n\t\"parentCategory\": null\r\n}";
                var content = new StringContent(category, Encoding.UTF8, "application/json");
                Console.WriteLine($"Calling {addCategoryUrl}");
                var response = client.PostAsync(addCategoryUrl, content).Result;

                Console.WriteLine(response.IsSuccessStatusCode
                    ? "Successfully added new category using Manager client app"
                    : "Failed to add new category from Manager client app");

                Console.WriteLine($"Calling {getCategoriesUrl}");
                var categoriesJson = client.GetStringAsync(getCategoriesUrl).Result;
                Console.WriteLine($"Server response:\r\n{categoriesJson}");
            }
            else
            {
                Console.WriteLine("Failed to acquire token!");
            }
        }

        private static void TestCatalogServiceAuthorization()
        {
            TestBuyerAuthorization();
            TestManagerAuthorization();
        }
    }
}