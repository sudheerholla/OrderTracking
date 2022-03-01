using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.ResponseModels.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderTracking.Application.Orders.Commands
{
    public class OrderOutForDeliveryCommand : IRequest<GetOrderDto>
    {
        public string Id { get; set; }

        public class OrderOutForDeliveryCommandHandler : IRequestHandler<OrderOutForDeliveryCommand, GetOrderDto>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;
            private IBusEndpoint bus;
            private readonly IBusEndpointFactory _busFactory;

            public OrderOutForDeliveryCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IBusEndpointFactory busFactory)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _busFactory = busFactory;
                this.bus = busFactory.Create("ordertracking.domain.entities.orderoutfordelivery");
            }

            public async Task<GetOrderDto> Handle(OrderOutForDeliveryCommand request, CancellationToken cancellationToken)
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(_ => _.Id == request.Id);
                if (order == null) return null;

                await this.bus.SendAsync(order);
                return _mapper.Map<GetOrderDto>(order);
            }
        }
    }
}
