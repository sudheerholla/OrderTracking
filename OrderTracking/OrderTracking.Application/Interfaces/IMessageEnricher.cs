using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace OrderTracking.Application.Interfaces
{
    public interface IMessageEnricher
    {
        Task EnrichAsync(ServiceBusMessage message);
    }
}