using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Mango.Web.Utils;

namespace Mango.Web.Services;

public class CouponService : ICouponService
{
    private readonly IBaseService _baseService;
    
    public CouponService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    public async Task<ResponseDto?> GetCouponsAsync()
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{SD.CouponAPIBase}"
        });
    }

    public async Task<ResponseDto?> GetCouponAsync(int id)
    {
        return await _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.GET,
            Url = $"{SD.CouponAPIBase}/{id}"
        });
    }

    public Task<ResponseDto?> CreateEditCouponAsync(CouponDto couponDto)
    {
        var httpMethod = couponDto.Id == 0 ? SD.ApiType.POST : SD.ApiType.PUT;
        var url = couponDto.Id == 0 ? SD.CouponAPIBase : $"{SD.CouponAPIBase}{couponDto.Id}";
        
        return _baseService.SendAsync(new()
        {
            ApiType = httpMethod,
            Data = couponDto,
            Url = url
        });
    }

    public Task<ResponseDto?> DeleteCouponAsync(int id)
    {
        return _baseService.SendAsync(new()
        {
            ApiType = SD.ApiType.DELETE,
            Url = $"{SD.CouponAPIBase}/{id}"
        });
    }
}