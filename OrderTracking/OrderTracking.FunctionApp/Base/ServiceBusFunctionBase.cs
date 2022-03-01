using MediatR;
using Microsoft.Azure.WebJobs;
using OrderTracking.Application.Interfaces;
using System.Threading.Tasks;

namespace CleanFunc.FunctionApp.Base
{
    public class ServiceBusFunctionBase
    {
        private readonly IMediator mediator;
        private readonly ICallContext context;

        protected ServiceBusFunctionBase(IMediator mediator, ICallContext context)
        {
            this.mediator = mediator;
            this.context = context;
        }

        protected async Task ExecuteAsync<TRequest>(ExecutionContext executionContext,
                                                                    TRequest request)
            where TRequest : IRequest
        {

            this.context.CorrelationId = executionContext.InvocationId;
            this.context.FunctionName = executionContext.FunctionName;

            await mediator.Send(request);
        }
    }
}