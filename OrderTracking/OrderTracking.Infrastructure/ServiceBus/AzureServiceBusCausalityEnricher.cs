using System.Threading.Tasks;
using OrderTracking.Application.Interfaces;
using Azure.Messaging.ServiceBus;

namespace OrderTracking.Infrastructure.ServiceBus
{
    // This enricher is used to enrich the service bus message with context information to correlate calls between 
    // azure functions and service bus. It is only required when azure service bus output bindings are not used because
    // normally the bindings would do this work.
    public class AzureServiceBusCausalityEnricher : IMessageEnricher
    {
        private readonly ICallContext callContext;

        public AzureServiceBusCausalityEnricher(ICallContext context)
        {
            this.callContext = context;
        }

        public Task EnrichAsync(ServiceBusMessage message)
        {
            message.ApplicationProperties.Add("$AzureWebJobsParentId", this.callContext.CorrelationId);
            return Task.CompletedTask;
        }
    }
}