using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.Orders.Queries;
using OrderTracking.Application.ResponseModels.QueryModels;
using OrderTracking.FunctionApp.Base;
using System.Threading.Tasks;

namespace OrderTracking.FunctionApp.Order
{
    public class GetOrderFunction : HttpFunctionBase
    {
        public GetOrderFunction(IMediator mediator,
                               ICallContext callContext)
           : base(mediator,
                   callContext)
        {
        }

        [FunctionName("GetOrder")]
        public async Task<IActionResult> GetOrder([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{id}")] GetOrderQuery queryArg, HttpRequest req, Microsoft.Azure.WebJobs.ExecutionContext context
            )

        {
            return await ExecuteAsync<GetOrderQuery, GetOrderDto>(context,
                                                                                req,
                                                                                queryArg,
                                                                                (r) => new OkObjectResult(r).ToTask());

        }
    }
}
