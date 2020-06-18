using System;
using Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace MessageProcessorFunction
{
    public static class MessageProcessor
    {
        [FunctionName("MessageProcessor")]
        public static void Run([ServiceBusTrigger("messages", Connection = "messageServiceBusConnectionString")]string messageTrigger, ILogger log)
        {
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageModel>(messageTrigger);


            log.LogInformation($"C# ServiceBus queue trigger function processed message: {messageTrigger}");




        }
    }
}
