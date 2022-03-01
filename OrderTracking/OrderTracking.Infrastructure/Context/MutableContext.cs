using OrderTracking.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Infrastructure.Context
{
    public class MutableCallContext : ICallContext
    {
        public Guid CorrelationId { get; set; }
        public string UserName { get; set; }
        public string AuthenticationType { get; set; }
        public string FunctionName { get; set; }
        public IDictionary<string, string> AdditionalProperties { get; } = new Dictionary<string, string>();
    }
}
