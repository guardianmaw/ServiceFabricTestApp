using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerServiceApp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceBusService _serviceBusService;
        private int _count = 0;

        public Worker(ILogger<Worker> logger, IServiceBusService serviceBusService)
        {
            _logger = logger;
            _serviceBusService = serviceBusService ?? throw new ArgumentNullException(nameof(serviceBusService));
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _serviceBusService.Dispose();

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _serviceBusService.Register(ProcessMessageAsync, new MessageHandlerOptions(MessageExceptionRecievedAsync) 
            {
                MaxConcurrentCalls = 100
            });


            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now}. Processed {_count} messages");
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            var model = JsonSerializer.Deserialize<MessageModel>(message.Body);
            _count++;

            //Simulate work
            await Task.Delay(5000);

            _logger.LogInformation($"Message recieved! Title: {model.Title}", model);

        }

        private Task MessageExceptionRecievedAsync(ExceptionReceivedEventArgs args)
        {
            //Ignore locked messages
            //if (args.Exception is MessageLockLostException)
            //{
            //    return Task.CompletedTask;
            //}

            _logger.LogError($"Exception Recieved from Service Bus Message Handler: {args.Exception.ToString()}", args.Exception);
            return Task.CompletedTask;
        }
    }
}
