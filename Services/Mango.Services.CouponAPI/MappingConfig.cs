using AutoMapper;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;

namespace Mango.Services.CouponAPI;

// public class MappingConfig: Profile
// {
//     public MappingConfig()
//     {
//         CreateMap<Coupon, CouponDto>();
//         CreateMap<CouponDto, Coupon>();
//     }
// }

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Coupon, CouponDto>();
            config.CreateMap<CouponDto, Coupon>();
        });

        return mappingConfig;
    }
}