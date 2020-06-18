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
    [ActorService(Name = "SMSProcessorActorService")]
    public class SMSProcessor : Actor, ISMSProcessor
    {
        public SMSProcessor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }

        public async Task<bool> SendSMSAsync(MessageModel messageModel)
        {
            await Task.Delay(1000);
            return true;
        }
    }
}
