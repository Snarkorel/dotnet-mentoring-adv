using Azure.Messaging.ServiceBus;

namespace Infrastructure.ServiceBus
{
    public abstract class MessagingClient
    {
        //TODO: move connection string to config
        protected const string ConnectionString = "CHANGE_ME";
        //Note: using single hardcoded value because task involves only one queue
        protected const string QueueName = "catalog-changes";

        protected readonly ServiceBusClient Client;

        protected MessagingClient()
        {
            Client = new ServiceBusClient(ConnectionString);
        }
    }
}