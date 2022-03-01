using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Orders.Commands;
using OrderTracking.Application.ResponseModels.QueryModels;
using OrderTracking.FunctionApp.Base;
using System.Threading.Tasks;

namespace OrderTracking.FunctionApp.Order
{
    public class OrderAcceptedFunction : HttpFunctionBase
    {

        public OrderAcceptedFunction(IMediator mediator,
                              ICallContext callContext)
          : base(mediator,
                  callContext)
        {
        }
        [FunctionName("OrderAccepted")]
        public async Task<IActionResult> OrderAccepted([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/accepted/{id}")] OrderAcceptedCommand queryArg, HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            return await ExecuteAsync<OrderAcceptedCommand, GetOrderDto>(context,
                                                                                req,
                                                                                queryArg,
                                                                                (r) => new OkObjectResult(r).ToTask());

        }
    }
}
