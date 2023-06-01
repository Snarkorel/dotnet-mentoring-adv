using CartingService.Grpc;
using Grpc.Core;
using System.Globalization;
using System.IO;
using CartingService.Core.Entities;
using CartingService.Core.Interfaces;
using Google.Protobuf.Collections;

namespace CartingService.Grpc.Services
{
    public class CartingGrpcService : Carting.CartingBase
    {
        private readonly ILogger<CartingGrpcService> _logger;
        private readonly ICartingService _cartingService;
        public CartingGrpcService(ILogger<CartingGrpcService> logger, ICartingService cartingService)
        {
            _logger = logger;
            _cartingService = cartingService;
        }

        //public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        //{
        //    return Task.FromResult(new HelloReply
        //    {
        //        Message = "Hello " + request.Name
        //    });
        //}

        //TODO!

        public override async Task<GetItemsReply> GetItems(GetItemsRequest request, ServerCallContext context)
        {
            var cart = await _cartingService.GetCartInfo(request.CartName);
            var items = cart.Items;

            var convertedItems = ConvertItems(items);

            var response = new GetItemsReply();
            response.Item.AddRange(convertedItems);

            return response;
        }

        public override Task GetItemsStream(GetItemsRequest request, IServerStreamWriter<GetItemsReply> responseStream, ServerCallContext context)
        {
            return base.GetItemsStream(request, responseStream, context);
        }

        public override Task<AddItemRequest> AddItem(IAsyncStreamReader<AddItemRequest> requestStream, ServerCallContext context)
        {
            return base.AddItem(requestStream, context);
        }

        public override Task AddItemStream(IAsyncStreamReader<AddItemRequest> requestStream, IServerStreamWriter<AddItemRequest> responseStream, ServerCallContext context)
        {
            return base.AddItemStream(requestStream, responseStream, context);
        }

        private static decimal GetDecimal(string value)
        {
            return decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        private RepeatedField<Item> ConvertItems(IEnumerable<CartItem> items)
        {
            var convertedItems = new RepeatedField<Item>();
            foreach (var item in items)
            {
                convertedItems.Add(new Item
                {
                    Id = item.Id,
                    Image = item.Image.ToString(),
                    Name = item.Name,
                    Price = item.Price.ToString(CultureInfo.InvariantCulture),
                    Quantity = item.Quantity,
                });
            }

            return convertedItems;
        }
    }
}