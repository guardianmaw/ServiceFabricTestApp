using Core;
using MessageProcessor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor
{
    [StatePersistence(StatePersistence.Volatile)]
    [ActorService(Name = "PushProcessorActorService")]
    public class PushProcessor : Actor, IPushProcessor
    {
        public PushProcessor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public async Task<bool> SendPushAsync(MessageModel messageModel)
        {
            await Task.Delay(1000);
            return true;
        }
    }
}
