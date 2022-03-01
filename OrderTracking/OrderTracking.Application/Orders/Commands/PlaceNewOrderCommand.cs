using AutoMapper;
using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using OrderTracking.Application.Interfaces;
using OrderTracking.Domain.Entities;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderTracking.Application.Orders.Commands
{
    public class PlaceNewOrderCommand : IRequest<bool>
    {
       
        public Order Model { get; set; } = new Order();

        public class PlaceNewOrderCommandHandler : IRequestHandler<PlaceNewOrderCommand, bool>
        {
            private readonly IBusEndpoint bus;
            private readonly IBusEndpointFactory _busFactory;
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;
           

            public PlaceNewOrderCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IBusEndpointFactory busFactory)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _busFactory = busFactory;
                this.bus = busFactory.Create<Order>();
            }

            public async Task<bool> Handle(PlaceNewOrderCommand request, CancellationToken cancellationToken)
            {
                var entity = new Order
                {
                    Id = new Random().Next(1,100000).ToString(),
                    RestaurantId = request.Model.RestaurantId,
                    RestaurantName = request.Model.RestaurantName,
                    Customer = request.Model.Customer,
                    OrderItems = request.Model.OrderItems,
                    OrderStatus = Domain.Enums.Enums.OrderStatus.New
                };

                _dbContext.Orders.Add(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);
                await this.bus.SendAsync(entity);
               

                return true;
            }
        }
    }
}
