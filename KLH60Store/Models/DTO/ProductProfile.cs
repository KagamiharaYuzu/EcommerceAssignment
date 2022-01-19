using AutoMapper;
using StoreClassLibrary;

namespace KLH60Store.Models.DTO
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductCategory, ProductDTO>()
                .ForMember(d=>d.CategoryName, opt => opt.MapFrom(s => s.ProdCat));
        }
    }
}