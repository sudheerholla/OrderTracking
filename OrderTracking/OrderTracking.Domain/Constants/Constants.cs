using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Domain.Constants
{
    public class Constants
    {
        public const string RESTAURANT_ORDER_ACCEPT_EVENT = "OrderAccepted";
        public const string RESTAURANT_ORDER_OUTFORDELIVERY_EVENT = "OrderOutForDelivery";
        public const string DELIVERY_ORDER_DELIVERED_EVENT = "OrderDelivered";
    }
}
