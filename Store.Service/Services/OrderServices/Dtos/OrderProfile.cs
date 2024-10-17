using AutoMapper;
using Store.Date.Entities.OrderEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.OrderServices.Dtos
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<ShippingAdress, AddressDto>().ReverseMap();
            CreateMap<Order, OrderDetailsdto>()
                .ForMember(dest => dest.DeliveryMethodName, options => options.MapFrom(src => src.DeliveryMethod.ShortName))
                .ForMember(dest => dest.ShippingPrice, options => options.MapFrom(src => src.DeliveryMethod.Price))
                .ForMember(dest => dest.ShippingAddress, options => options.MapFrom(src => src.ShippingAdress));
            CreateMap<OrderItem, OrderItemDto>()
             .ForMember(dest => dest.ProductItemId, options => options.MapFrom(src => src.ProductItem.ProductId))
             .ForMember(dest => dest.ProductName, options => options.MapFrom(src => src.ProductItem.ProductName))
             .ForMember(dest => dest.PictureUrl, options => options.MapFrom(src => src.ProductItem.PictureUrl))
             .ForMember(dest => dest.PictureUrl, options => options.MapFrom<OrderItemPictureUrlResolver>()).ReverseMap();

        }
    }
}
