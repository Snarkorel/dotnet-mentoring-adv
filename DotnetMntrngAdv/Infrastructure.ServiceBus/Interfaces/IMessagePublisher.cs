using Infrastructure.ServiceBus.DTO;

namespace Infrastructure.ServiceBus.Interfaces
{
    public interface IMessagePublisher
    {
        Task Send(ItemDto item);
    }
}
