using Azure.Messaging.ServiceBus;
using System.Text.Json;
using Infrastructure.ServiceBus.Interfaces;
using Infrastructure.ServiceBus.DTO;

namespace Infrastructure.ServiceBus
{
    public class MessagingClient : IMessagingClient
    {
        //TODO: move connection string to config
        private const string ConnectionString = "CHANGE_ME";
        //Note: using single hardcoded value because task involves only one queue
        private const string QueueName = "catalog-changes";

        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ServiceBusReceiver _receiver;

        public MessagingClient()
        {
            _client = new ServiceBusClient(ConnectionString);
            _sender = _client.CreateSender(QueueName);
            _receiver = _client.CreateReceiver(QueueName);
        }

        public async Task Send(ItemDto dto)
        {
            var payload = JsonSerializer.Serialize(dto);
            var message = new ServiceBusMessage(payload);

            await _sender.SendMessageAsync(message);
        }

        public async Task<ItemDto?> Receive()
        {
            var message = await _receiver.ReceiveMessageAsync();
            var payload = message.Body.ToString();
            if (string.IsNullOrEmpty(payload))
                return null;

            var dto = JsonSerializer.Deserialize<ItemDto>(payload);
            return dto ?? null;
        }
    }
}