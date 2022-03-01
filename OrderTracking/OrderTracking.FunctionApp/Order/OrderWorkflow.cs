using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderTracking.Domain.Entities;
using System.Net.Http;
using System.Threading;
using OrderTracking.Domain.Constants;
using OrderTracking.Application.ResponseModels.QueryModels;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using OrderTracking.Application.Interfaces;

namespace OrderTracking.FunctionApp.Order
{
    public class OrderWorkflow
    {

        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _dbContext;
        public OrderWorkflow(IConfiguration configuration, IApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        [FunctionName("OrderPlacedOrchestrator")]
        public async Task OrderPlacedOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context,
           ILogger log)
        {

            log = context.CreateReplaySafeLogger(log);
            var order = context.GetInput<OrderTracking.Domain.Entities.Order>();

            try
            {
                await context.CallActivityAsync("NotifyRestaurant", order);

                Uri uri = new Uri($"{_configuration["HostEndpoint"]}/orders/accepted/{order.Id}");

                await context.CallHttpAsync(HttpMethod.Get, uri);

                using (var cts = new CancellationTokenSource())
                {
                    var timeoutAt = context.CurrentUtcDateTime.AddSeconds(180);// if more than 3 mins cancel order
                    var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);
                    var acknowledgeTask = context.WaitForExternalEvent(Constants.RESTAURANT_ORDER_ACCEPT_EVENT);
                    var winner = await Task.WhenAny(acknowledgeTask, timeoutTask);

                    if (winner == acknowledgeTask)
                    {
                        string instanceId = $"{order.Id}-accepted";

                        context.StartNewOrchestration("OrderAcceptedOrchestrator", order, instanceId);

                        cts.Cancel();
                    }
                    else
                    {
                       
                        log.LogError($"OrderPlacedOrchestrator Timed out {order.Id}");
                        order.OrderStatus = Domain.Enums.Enums.OrderStatus.Canceled;
                        await context.CallActivityAsync("UpsertOrder", order);
                        await context.CallActivityAsync("NotifyCustomer", order);

                        await context.CallActivityAsync("NotifyRestaurant", order);

                        cts.Cancel();
                    }
                }
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

        [FunctionName("OrderAcceptedOrchestrator")]
        public async Task OrderAcceptedOrchestrator(
           [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log); 
            var order= context.GetInput<OrderTracking.Domain.Entities.Order>();

            try
            {
                order.OrderStatus = Domain.Enums.Enums.OrderStatus.Accepted;

                await context.CallActivityAsync("UpsertOrder", order);

                await context.CallActivityAsync("NotifyCustomer", order);

                Uri uri = new Uri($"{_configuration["HostEndpoint"]}/orders/outForDelivery/{order.Id}");

                await context.CallHttpAsync(HttpMethod.Get, uri);

                using (var cts = new CancellationTokenSource())
                {
                    var timeoutAt = context.CurrentUtcDateTime.AddHours(1);
                    var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);
                    var acknowledgeTask = context.WaitForExternalEvent(Constants.RESTAURANT_ORDER_OUTFORDELIVERY_EVENT);
                    var winner = await Task.WhenAny(acknowledgeTask, timeoutTask);

                    if (winner == acknowledgeTask)
                    {
                        string instanceId = $"{order.Id}-out-for-delivery";

                        context.StartNewOrchestration("OrderOutForDeliveryOrchestrator", order, instanceId);

                        cts.Cancel();
                    }
                    else
                    {
                        log.LogError($"OrderAcceptedOrchestrator Timed out {order.Id}");

                        await context.CallActivityAsync("NotifyCustomer", order);

                        await context.CallActivityAsync("NotifyRestaurant", order);
                    }
                }
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

        [FunctionName("OrderOutForDeliveryOrchestrator")]
        public async Task OrderOutForDeliveryOrchestrator(
     [OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log = context.CreateReplaySafeLogger(log); 
            var order = context.GetInput<OrderTracking.Domain.Entities.Order>();
            try
            {
                order.OrderStatus = Domain.Enums.Enums.OrderStatus.OutForDelivery;

                await context.CallActivityAsync("UpsertOrder", order);

                await context.CallActivityAsync("NotifyCustomer", order);

                Uri uri = new Uri($"{_configuration["HostEndpoint"]}/orders/delivered/{order.Id}");

                await context.CallHttpAsync(HttpMethod.Get, uri);

                using (var cts = new CancellationTokenSource())
                {
                    var timeoutAt = context.CurrentUtcDateTime.AddHours(1);
                    var timeoutTask = context.CreateTimer(timeoutAt, cts.Token);
                    var acknowledgeTask = context.WaitForExternalEvent(Constants.DELIVERY_ORDER_DELIVERED_EVENT);
                    var winner = await Task.WhenAny(acknowledgeTask, timeoutTask);

                    if (winner == acknowledgeTask)
                    {
                        order.OrderStatus = Domain.Enums.Enums.OrderStatus.Delivered;

                        await context.CallActivityAsync("UpsertOrder", order);

                        await context.CallActivityAsync("NotifyCustomer", order);

                        cts.Cancel();
                    }
                    else
                    {
                       log.LogError($"OrderOutForDeliveryOrchestrator Timed out {order.Id}");

                        await context.CallActivityAsync("NotifyCustomer", order);

                        await context.CallActivityAsync("NotifyRestaurant", order);
                    }
                }
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

        [FunctionName("UpsertOrder")]
        public static void UpsertOrder([ActivityTrigger] OrderTracking.Domain.Entities.Order order,
       [CosmosDB(ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient client)
        {


            Uri ordersCollectionUri = UriFactory.CreateDocumentCollectionUri(databaseId: "OrderDb", collectionId: "Order");
            var options = new FeedOptions { EnableCrossPartitionQuery = true }; // Enable cross partition query
            var record =
                client.CreateDocumentQuery<Document>(ordersCollectionUri, options).Where(r => r.Id == order.Id).AsEnumerable()
                .SingleOrDefault(); 
            record.SetPropertyValue("OrderStatus", order.OrderStatus);
            var result = client.ReplaceDocumentAsync(record.SelfLink, record).ContinueWith(response =>
            {
                
            }); ;
            //Notify Customer
        }

        [FunctionName("NotifyRestaurant")]
        public static void NotifyRestaurant([ActivityTrigger] OrderTracking.Domain.Entities.Order order,
            ILogger log)
        {

            //Send notification to restaurant (may be through Notification Hub)

        }

        [FunctionName("NotifyCustomer")]
        public static void NotifyCustomer([ActivityTrigger] OrderTracking.Domain.Entities.Order order,
              ILogger log)
        {
            // Send notification to customer (maybe through Notification Hub etc)
        }
    }
}
