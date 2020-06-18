using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using MessageProcessor.Interfaces;
using Core;

namespace MessageProcessor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name = "MessageProcessorActorService")]
    internal class MessageProcessor : Actor, IMessageProcessor
    {
        /// <summary>
        /// Initializes a new instance of MessageProcessor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public MessageProcessor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }


        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        public async Task<bool> ProcessMessageAsync(MessageModel message)
        {
            var messageType = message.MessageType;
            IList<Task> tasks = new List<Task>();


            if ((messageType & MessageType.Email) == MessageType.Email)
            {
                IEmailProcessor processor = ActorProxy.Create<IEmailProcessor>(ActorId.CreateRandom(), applicationName: "ServiceFabricTestApp", serviceName: "EmailProcessorActorService");

                tasks.Add(processor.SendEmailAsync(message));

            }

            if ((messageType & MessageType.Push) == MessageType.Push)
            {
                IPushProcessor processor = ActorProxy.Create<IPushProcessor>(ActorId.CreateRandom(), applicationName: "ServiceFabricTestApp", serviceName: "PushProcessorActorService");

                tasks.Add(processor.SendPushAsync(message));
            }

            if ((messageType & MessageType.SMS) == MessageType.SMS)
            {
                ISMSProcessor processor = ActorProxy.Create<ISMSProcessor>(ActorId.CreateRandom(), applicationName: "ServiceFabricTestApp", serviceName: "SMSProcessorActorService");

                tasks.Add(processor.SendSMSAsync(message));
            }

            await Task.WhenAll(tasks);




            return true;
        }



    }
}
