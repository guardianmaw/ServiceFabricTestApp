using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using MessageProcessor.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Newtonsoft.Json.Linq;

namespace QueueSupervisor
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class QueueSupervisor : StatelessService
    {
        IServiceBusService _serviceBusService;

        public QueueSupervisor(StatelessServiceContext context, IServiceBusService serviceBusService)
            : base(context)
        {
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));

            ServiceEventSource.Current.ServiceMessage(this.Context, "New Stateless Supervisor Created", _serviceBusService);
            _serviceBusService.Register(ProcessMessageAsync, new MessageHandlerOptions(MessageExceptionRecievedAsync));

        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic 
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    long iterations = 0;

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}



        private async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            ServiceEventSource.Current.ServiceMessage(this.Context, "MESSAGE RECIEVED!!!", message);
            var model = JsonSerializer.Deserialize<MessageModel>(message.Body);

            IMessageProcessor actor = ActorProxy.Create<IMessageProcessor>(ActorId.CreateRandom(), new Uri("fabric:/ServiceFabricTestApp/MessageProcessorActorService"));
            var result = await actor.ProcessMessageAsync(model);


            if (result)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Message {model.Title} has been processed successfully", message);
                await _serviceBusService.CompleteAsync(message);
            }
            else
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"An error ocurred while processing the message with Id: {message.MessageId} and will be deadlettered", message);
                await _serviceBusService.DeadLetterAsync(message, "result was false");
            }
        }

        private Task MessageExceptionRecievedAsync(ExceptionReceivedEventArgs args)
        {
            //Ignore locked messages
            if (args.Exception is MessageLockLostException)
            {
                return Task.CompletedTask;
            }

            ServiceEventSource.Current.ServiceMessage(this.Context, $"Exception Recieved from Service Bus Message Handler: {args.Exception.ToString()}", args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}
