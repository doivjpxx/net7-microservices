using AutoMapper;

namespace Mango.Services.CouponAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        MapperConfiguration mappingConfig = new(cfg =>
        {
            cfg.CreateMap<Models.Coupon, Models.Dto.CouponDto>().ReverseMap();
            cfg.CreateMap<Models.Dto.CouponDto, Models.Dto.ResponseDto>().ReverseMap();
        });

        return mappingConfig;
    }
}
