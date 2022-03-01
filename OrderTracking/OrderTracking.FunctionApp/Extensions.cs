using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.FunctionApp
{
    public static class Extensions
    {
        public static Task<IActionResult> ToTask(this IActionResult result)
        {
            return Task.FromResult(result);
        }
    }

    public static class ExceptionHandler
    {
        public static void LogExceptionDetails(this Exception exception, ILogger log, string orderId, string className)
        {
            log.LogError($"Something went wrong in {className}.\r\nOrder ID: {orderId}.\r\nException details: {exception.ToString()}");
        }
    }
}
