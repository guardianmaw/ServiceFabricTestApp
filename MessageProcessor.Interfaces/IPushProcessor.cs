using Core;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor.Interfaces
{
    public interface IPushProcessor : IActor
    {
        public Task<bool> SendPushAsync(MessageModel messageModel);
    }
}
