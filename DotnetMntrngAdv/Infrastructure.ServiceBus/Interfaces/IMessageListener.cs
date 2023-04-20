using Infrastructure.ServiceBus.DTO;

namespace Infrastructure.ServiceBus.Interfaces
{
    public interface IMessageListener
    {
        Task Subscribe(Func<ItemDto> handler);
    }
}
