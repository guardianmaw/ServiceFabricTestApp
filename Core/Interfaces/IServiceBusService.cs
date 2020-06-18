using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IServiceBusService : IDisposable
    {
        Task AbandonAsync(Message message);
        Task AddMessageAsync(Message message);
        Task AddMessagesAsync(IList<Message> messages);
        Task CompleteAsync(Message message);
        Task DeadLetterAsync(Message message, string deadLetterReason, string deadLetterErrorDescription = null);
        void Register(Func<Message, CancellationToken, Task> handler, MessageHandlerOptions options);
    }
}
