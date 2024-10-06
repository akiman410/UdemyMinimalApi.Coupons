using AutoMapper;
using UdemyMinimalApi.Coupons.Models;
using UdemyMinimalApi.Coupons.Models.DTOs;

namespace UdemyMinimalApi.Coupons
{
    public class MappingConfig:Profile
    {
        public MappingConfig() 
        { 
            CreateMap<Coupon,CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<LocalUser, UserDTO>().ReverseMap();
        }
    }
}
