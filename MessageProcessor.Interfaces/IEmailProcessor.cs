using Core;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessageProcessor.Interfaces
{
    public interface IEmailProcessor : IActor
    {
        public Task<bool> SendEmailAsync(MessageModel message);
    }
}
