using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Application.Interfaces
{
    public interface ICallContext
    {
        Guid CorrelationId
        {
            get;
            set;
        }

        string FunctionName
        {
            get;
            set;
        }

        string UserName
        {
            get;
            set;
        }

        string AuthenticationType
        {
            get;
            set;
        }

        IDictionary<string, string> AdditionalProperties
        {
            get;
        }
    }
}
