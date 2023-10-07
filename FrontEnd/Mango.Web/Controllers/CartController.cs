using System.IdentityModel.Tokens.Jwt;
using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View(await LoadCartDtoBasedOnLoggedInUser());
    }

    private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
    {
        var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
        var response = await _cartService.GetCartByUserIdAsync(userId);
        if (response is null || !response.IsSuccess)
        {
            return new CartDto();
        }
        
        return JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
    }
}