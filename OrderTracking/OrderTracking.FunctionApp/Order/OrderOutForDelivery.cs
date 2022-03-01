using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.ResponseModels.QueryModels;
using OrderTracking.FunctionApp;
using OrderTracking.FunctionApp.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Application.Orders.Commands
{
    public class OrderOutForDeliveryFunction : HttpFunctionBase
    {
        public OrderOutForDeliveryFunction(IMediator mediator,
                             ICallContext callContext)
         : base(mediator,
                 callContext)
        {
        }
        [FunctionName("OrderOutForDelivery")]
        public async Task<IActionResult> OrderOutForDelivery([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/outfordelivery/{id}")] OrderOutForDeliveryCommand queryArg, HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            return await ExecuteAsync<OrderOutForDeliveryCommand, GetOrderDto>(context,
                                                                                req,
                                                                                queryArg,
                                                                                (r) => new OkObjectResult(r).ToTask());

        }
    }
}
