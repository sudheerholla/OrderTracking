using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Orders.Commands;
using OrderTracking.Application.ResponseModels.QueryModels;
using OrderTracking.FunctionApp.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.FunctionApp.Order
{
    public class OrderDeliveredFunction : HttpFunctionBase
    {
        public OrderDeliveredFunction(IMediator mediator,
                             ICallContext callContext)
         : base(mediator,
                 callContext)
        {
        }
        [FunctionName("OrderDelivered")]
        public async Task<IActionResult> OrderDelivered([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/delivered/{id}")] OrderDeliveredCommand queryArg, HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            return await ExecuteAsync<OrderDeliveredCommand, GetOrderDto>(context,
                                                                                req,
                                                                                queryArg,
                                                                                (r) => new OkObjectResult(r).ToTask());

        }
    }
}
