using System.Collections.Generic;

namespace OrderTracking.Application.Interfaces
{
    public interface IServiceBusConfiguration
    {
        public string DefaultConnectionString {get;}
        public Dictionary<string, string> OtherConnectionStrings {get;}
    }
}