using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderTracking.Domain.Entities;
using OrderTracking.Domain.Constants;

namespace OrderTracking.FunctionApp.Order
{
    public class OrderWorkflowTrigger
    {

        [Microsoft.Azure.WebJobs.FunctionName("PlaceNewOrderQueueTrigger")]
        public async Task PlaceNewOrderQueueTrigger(
       [DurableClient] IDurableOrchestrationClient context,
       [ServiceBusTrigger("%NewOrderQueue%", Connection = "ServiceBusconnectionString")] OrderTracking.Domain.Entities.Order order,
       ILogger log)
        {
            try
            {
                string instanceId = order.Id.ToString();
                await StartInstance(context, order, instanceId, log);
            }
            catch (Exception ex)
            {
                if (order != null)
                {
                    ex.LogExceptionDetails(log, order.Id, GetType().FullName);
                }
                else
                {
                    ex.LogExceptionDetails(log, null, GetType().FullName);
                }
            }
        }

        [FunctionName("AcceptedOrderQueueTrigger")]
        public async Task AcceptedOrderQueueTrigger(
       [DurableClient] IDurableOrchestrationClient context,
       [ServiceBusTrigger("%OrderAcceptedQueue%", Connection = "ServiceBusconnectionString")] OrderTracking.Domain.Entities.Order order,
       ILogger log)
        {
            try
            {
                string instanceId = order.Id.ToString();
                await context.RaiseEventAsync(instanceId, Constants.RESTAURANT_ORDER_ACCEPT_EVENT);
            }
            catch (Exception ex)
            {
                if (order != null)
                {
                    ex.LogExceptionDetails(log, order.Id, GetType().FullName);
                }
                else
                {
                    ex.LogExceptionDetails(log, null, GetType().FullName);
                }
            }
        }

        [FunctionName("OutForDeliveryOrderQueueTrigger")]
        public async Task OutForDeliveryOrderQueueTrigger(
        [DurableClient] IDurableOrchestrationClient context,
        [ServiceBusTrigger("%OrderOutForDeliveryQueue%", Connection = "ServiceBusconnectionString")] OrderTracking.Domain.Entities.Order order,
        ILogger log)
        {
            try
            {
                string instanceId = $"{order.Id}-accepted";
                await context.RaiseEventAsync(instanceId, Constants.RESTAURANT_ORDER_OUTFORDELIVERY_EVENT);
            }
            catch (Exception ex)
            {
                if (order != null)
                {
                    ex.LogExceptionDetails(log, order.Id, GetType().FullName);
                }
                else
                {
                    ex.LogExceptionDetails(log, null, GetType().FullName);
                }
            }

        }

        [FunctionName("DeliveredOrderQueueTrigger")]
        public async Task DeliveredOrderQueueTrigger(
        [DurableClient] IDurableOrchestrationClient context,
        [ServiceBusTrigger("%OrderDeliveredQueue%", Connection = "ServiceBusconnectionString")] OrderTracking.Domain.Entities.Order order,
        ILogger log)
        {
            try
            {
                string instanceId = $"{order.Id}-out-for-delivery";
                await context.RaiseEventAsync(instanceId, Constants.DELIVERY_ORDER_DELIVERED_EVENT);
            }
            catch (Exception ex)
            {
                if (order != null)
                {
                    ex.LogExceptionDetails(log, order.Id, GetType().FullName);
                }
                else
                {
                    ex.LogExceptionDetails(log, null, GetType().FullName);
                }
            }
        }


        private async Task StartInstance(IDurableOrchestrationClient context, OrderTracking.Domain.Entities.Order order, string instanceId, ILogger log)
        {
            try
            {
                var reportStatus = await context.GetStatusAsync(instanceId);
                string runningStatus = reportStatus == null ? "NULL" : reportStatus.RuntimeStatus.ToString();
                
                if (reportStatus == null || reportStatus.RuntimeStatus != OrchestrationRuntimeStatus.Running)
                {
                    await context.StartNewAsync("OrderPlacedOrchestrator", instanceId, order);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
