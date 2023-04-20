using Infrastructure.ServiceBus.DTO;

namespace Infrastructure.ServiceBus.Interfaces
{
    public interface IMessagingClient
    {
        Task Send(ItemDto item);

        Task<ItemDto?> Receive();
    }
}
