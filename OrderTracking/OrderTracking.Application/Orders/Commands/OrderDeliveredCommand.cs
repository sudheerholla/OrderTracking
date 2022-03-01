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
  
    public class OrderDeliveredCommand : IRequest<GetOrderDto>
    {
        public string Id { get; set; }

        public class OrderDeliveredCommandHandler : IRequestHandler<OrderDeliveredCommand, GetOrderDto>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;
            private IBusEndpoint bus;
            private readonly IBusEndpointFactory _busFactory;

            public OrderDeliveredCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IBusEndpointFactory busFactory)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _busFactory = busFactory;
                this.bus = busFactory.Create("ordertracking.domain.entities.orderdelivered");
            }

            public async Task<GetOrderDto> Handle(OrderDeliveredCommand request, CancellationToken cancellationToken)
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(_ => _.Id == request.Id);
                if (order == null) return null;

                await this.bus.SendAsync(order);
                return _mapper.Map<GetOrderDto>(order);
            }
        }
    }
}
