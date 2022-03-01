using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderTracking.Application.ResponseModels.QueryModels
{
    public class GetOrderDto
    {
        public string Id { get; set; }

        public string RestaurantId { get; set; }

        public string RestaurantName { get; set; }

        public Customer Customer { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public List<MenuItem> OrderItems { get; set; }
    }

    public class Customer
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public bool IsBlocked { get; set; }
    }

    public class OrderLite
    {
        public string Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

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

    public class MenuItem
    {
        public string Id { get; set; }

        public string Item { get; set; }

        public int Price { get; set; }

        public DishType DishType { get; set; }
    }
    public enum DishType
    {
        Veg = 0,
        NonVeg = 1,
        ContainsEgg = 2
    }

}
