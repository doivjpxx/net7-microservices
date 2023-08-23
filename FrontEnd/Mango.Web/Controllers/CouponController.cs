using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;
    
    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }
    
    public async Task<IActionResult> Index()
    {
        List<CouponDto>? list = new();
        
        var response = await _couponService.GetCouponsAsync();
        
        if (response is not null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
        }
        
        return View(list);
    }
    
    public async Task<IActionResult> Create()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CouponDto model)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response = await _couponService.CreateEditCouponAsync(model);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon created successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }
        return View(model);
    }
    
    public async Task<IActionResult> Delete(int couponId)
    {
        ResponseDto? response = await _couponService.GetCouponAsync(couponId);

        if (response != null && response.IsSuccess)
        {
            CouponDto? model= JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete(CouponDto couponDto)
    {
        ResponseDto? response = await _couponService.DeleteCouponAsync(couponDto.Id);

        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon deleted successfully";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(couponDto);
    }

}