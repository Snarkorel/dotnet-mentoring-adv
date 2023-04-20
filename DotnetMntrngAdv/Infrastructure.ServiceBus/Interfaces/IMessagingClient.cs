using CartingService.Core.Entities;
using CatalogService.Domain.Entities;

namespace Infrastructure.ServiceBus.Interfaces
{
    public interface IMessagingClient
    {
        Task Send(ProductItem item);

        Task<CartItem?> Receive();
    }
}
