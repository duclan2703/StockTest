using AutoMapper;
using Stock.Entity.DTOs;
using Stock.Entity.Entities;

namespace Stock.MapperProfile
{
    public class MapperProfile
    {
        public static IMapper Initialize()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SalesOrderDetail, OrderDetailDto>()
                    .ReverseMap();

                cfg.CreateMap<SalesOrderHeader, OrderDto>()
                    .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.SalesOrderDetails))
                .ReverseMap()
                    .ForMember(dest => dest.SalesOrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
            });
            return config.CreateMapper();
        }
    }
}
