using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Mango.Web.Utils;

namespace Mango.Web.Services;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;
    
    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ShoppingCartAPIBase + "/GetCart/" + userId,
        });
    }

    public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ShoppingCartAPIBase + "/CartUpsert",
            Data = cartDto,
        });
    }

    public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ShoppingCartAPIBase + "/RemoveCart",
            Data = cartDetailsId,
        });
    }

    public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = cartDto,
            Url = SD.ShoppingCartAPIBase + "/ApplyCoupon",
        });
    }

    public async Task<ResponseDto?> RemoveCouponAsync(CartDto cartDto)
    {
        return await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Url = SD.ShoppingCartAPIBase + "/RemoveCart",
            Data = cartDto,
        });
    }
}