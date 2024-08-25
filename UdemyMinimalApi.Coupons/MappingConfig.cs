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
            CreateMap<Coupon, CouponDTO>().ReverseMap();
        }
    }
}
