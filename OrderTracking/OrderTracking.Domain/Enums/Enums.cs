using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Domain.Enums
{
    public class Enums
    {
        public enum ItemTypes
        {
            Order,
            Customer,
            Restaurant
        }
        public enum OrderStatus
        {
            Unassigned,
            New,
            Accepted,
            OutForDelivery,
            Delivered,
            DeliveryFailed,
            Canceled
        }
    }
}
