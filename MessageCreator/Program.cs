using Core;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MessageCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceBusService = new ServiceBusService();

            var messages = new List<MessageModel>();

            for (int i = 0; i < 100000; i++)
            {

                messages.Add(new MessageModel()
                {
                    Body = $"Message {i} body.",
                    Title = $"Message {i}",
                    Destinations = new List<string> { "person@test.com" },
                    MessageType = MessageType.Email | MessageType.Push
                });
            }


            var batches = messages.Select((item, index) => new { item, index })
                                  .GroupBy(x => x.index / 1000)
                                  .Select(x => x.Select(y => new Message()
                                  {
                                      Body = JsonSerializer.SerializeToUtf8Bytes(y.item, typeof(MessageModel)),
                                      MessageId = Guid.NewGuid().ToString(),
                                      ContentType = "Application/Json"
                                  }));

            var count = 0;
            foreach (var msgs in batches)
            {
                Console.WriteLine($"Added batch {count++}");
                await serviceBusService.AddMessagesAsync(msgs.ToList()); ;
            }
        }
    }
}
