using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    
    public async Task<IActionResult> Index()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    public async Task<IActionResult> Remove(int cartDetailsId)
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
        
        if(response!=null & response.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
    {
        ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon is applied successfully";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
    {
        ResponseDto? response = await _cartService.RemoveCouponAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        return View();
    }
    
    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
        ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
        if(response!=null & response.IsSuccess)
        {
            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            return cartDto;
        }
        return new CartDto();
    }
}