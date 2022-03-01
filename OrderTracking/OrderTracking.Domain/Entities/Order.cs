using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using OrderTracking.Domain.Enums;
using static OrderTracking.Domain.Enums.Enums;
using System.ComponentModel.DataAnnotations;

namespace OrderTracking.Domain.Entities
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        [Key]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "restaurantId")]
        public string RestaurantId { get; set; }

        [JsonProperty(PropertyName = "restaurantName")]
        public string RestaurantName { get; set; }

        [JsonProperty(PropertyName = "customer")]
        public Customer Customer { get; set; }

        [JsonProperty(PropertyName = "orderStatus")]
        public OrderStatus OrderStatus { get; set; }

        [JsonProperty(PropertyName = "orderItems")]
        public List<MenuItem> OrderItems { get; set; }
    }
    

    public class OrderLite
    {
        public string Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
 
}
