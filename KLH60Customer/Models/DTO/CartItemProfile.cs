using AutoMapper;
using StoreClassLibrary;

namespace KLH60Customer.Models.DTO
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<CartItem, CartItemDTO>();
            CreateMap<Product, CartItemDTO>().ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
        }
    }
}