using Mango.Services.ShoppingCart.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Newtonsoft.Json;

namespace ShoppingCartAPI.Services;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CouponDto> GetCoupon(string couponCode)
    {
        var client = _httpClientFactory.CreateClient("Coupon");
        var response = await client.GetAsync($"/api/Coupon/code/{couponCode}");
        var apiContent = await response.Content.ReadAsStringAsync();
        var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (res.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(res.Result));
        }

        return null;
    }
}