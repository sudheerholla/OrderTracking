using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderTracking.Application.Interfaces;
using OrderTracking.Application.ResponseModels.QueryModels;
using OrderTracking.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace OrderTracking.Application.Orders.Queries
{
    public class GetOrderQuery : IRequest<GetOrderDto>
    {
        public string Id { get; set; }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<OrderTracking.Domain.Entities.MenuItem, ResponseModels.QueryModels.MenuItem>();
                CreateMap<OrderTracking.Domain.Entities.Customer, ResponseModels.QueryModels.Customer>();
                CreateMap<Order, GetOrderDto>()
                .ForMember(s => s.OrderItems, opt => opt.MapFrom(x => x.OrderItems))
                .ForMember(s => s.Customer, opt => opt.MapFrom(x => x.Customer))
                .ForMember(s => s.OrderStatus, opt => opt.MapFrom(x => x.OrderStatus));
            }
        }

        public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderDto>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public GetOrderQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<GetOrderDto> Handle(GetOrderQuery request, CancellationToken cancellationToken)
            {
                var order = await _dbContext.Orders.FirstOrDefaultAsync(_ => _.Id == request.Id);
                return _mapper.Map<GetOrderDto>(order);
            }
        }
    }
}
