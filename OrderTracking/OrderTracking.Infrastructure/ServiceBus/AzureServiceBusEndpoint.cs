using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using OrderTracking.Application.Interfaces;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace OrderTracking.Infrastructure.ServiceBus
{
    internal class AzureServiceBusEndpoint : IBusEndpoint, IAsyncDisposable
    {
        private readonly ServiceBusSender _messageSender;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly IEnumerable<IMessageEnricher> _enrichers;

        public AzureServiceBusEndpoint(IEnumerable<IMessageEnricher> enrichers, string connectionString, string queueOrTopicName)
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
            _messageSender = _serviceBusClient.CreateSender(queueOrTopicName);
            _enrichers = enrichers;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(_messageSender.CloseAsync());
        }

        public async Task SendAsync<TPayload>(TPayload payload)  where TPayload: class
        {
            string data = JsonConvert.SerializeObject(payload);
            ServiceBusMessage message = new ServiceBusMessage(Encoding.UTF8.GetBytes(data));
            message.ContentType = "application/json";

            // run each of the enrichers 
            foreach(var enricher in _enrichers)
                await enricher.EnrichAsync(message);

            await _messageSender.SendMessageAsync(message);    
        }   
    }

}