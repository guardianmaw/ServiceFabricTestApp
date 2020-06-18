using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Core
{
    public class ServiceBusService : IServiceBusService
    {
        private IQueueClient _serviceBusClient;
        private const string _connectionString = "Endpoint=sb://michealwheeler.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=zSOCaee9APoXNUUNRNsC1vxFV8/bFWGvvoypExExBok=";
        private const string _queueName = "messages";
        public ServiceBusService()
        {
            var retryPolicy = new RetryExponential(TimeSpan.FromSeconds(0.2), TimeSpan.FromSeconds(30), 5);

            _serviceBusClient = new QueueClient(_connectionString, _queueName, ReceiveMode.PeekLock, retryPolicy);
        }

        public void Register(Func<Message, CancellationToken, Task> handler,  MessageHandlerOptions options)
        {
            _serviceBusClient.RegisterMessageHandler(handler, options);
        }

        public async Task CompleteAsync(Message message)
        {
            await _serviceBusClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        public async Task DeadLetterAsync(Message message, string deadLetterReason, string deadLetterErrorDescription = null)
        {
            await _serviceBusClient.DeadLetterAsync(message.SystemProperties.LockToken, deadLetterReason, deadLetterErrorDescription);
        }

        public async Task AbandonAsync(Message message)
        {
            await _serviceBusClient.AbandonAsync(message.SystemProperties.LockToken);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _serviceBusClient.SendAsync(message);
        }

        public async Task AddMessagesAsync(IList<Message> messages)
        {
            await _serviceBusClient.SendAsync(messages);
        }

        public void Dispose()
        {
            _serviceBusClient.CloseAsync().GetAwaiter().GetResult();
        }
    }
}
