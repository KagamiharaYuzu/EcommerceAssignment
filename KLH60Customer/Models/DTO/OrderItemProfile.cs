using AutoMapper;
using StoreClassLibrary;

namespace KLH60Customer.Models.DTO
{
    public class OrderItemProfile : Profile
    {
        public OrderItemProfile()
        {
            CreateMap<OrderItem, OrderItemDTO>();
            CreateMap<Product, OrderItemDTO>()
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
        }
    }
}