using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Orders.Commands;
using OrderTracking.FunctionApp.Base;
using OrderTracking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.FunctionApp.Order
{
    public class PlaceOrder : HttpFunctionBase
    {
        public PlaceOrder(IMediator mediator, ICallContext context) : base(mediator, context)
        {
        }

        [FunctionName("PlaceOrder")]
        public async Task<ActionResult> PlaceNewOrder([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")] Domain.Entities.Order queryArg,
        HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            var command = new PlaceNewOrderCommand()
            {
                Model = queryArg
            };
            return (ActionResult)await ExecuteAsync<PlaceNewOrderCommand, bool>(context,
                                                                               req,
                                                                               command,
                                                                               (r) => new OkObjectResult(r).ToTask());
        }
    }
}
