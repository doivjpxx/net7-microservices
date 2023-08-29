using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CouponController : ControllerBase
{
    private readonly AppDbContext _db;
    private IMapper _mapper;
    
    public CouponController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<ResponseDto> GetCouponsAsync()
    {
        var coupons = await _db.Coupons.ToListAsync();
        var res = _mapper.Map<List<CouponDto>>(coupons);
        
        return res.Count == 0
            ? new ResponseDto {IsSuccess = false, Message = "No Coupons Exists"}
            : new ResponseDto {IsSuccess = true, Result = res};
    }
    
    [HttpGet("{id:int}")]
    public async Task<ResponseDto> GetByIdAsync(int id)
    {
        var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == id);

        var res = _mapper.Map<CouponDto>(couponFromDb);

        return new ResponseDto
        {
            Result = res,
            IsSuccess = couponFromDb != null,
            Message = couponFromDb == null ? "Coupon Not Found" : "Coupon Found"
        };
    }
    
    [HttpGet("code/{couponCode}")]
    public async Task<ResponseDto> GetDiscountAsync(string couponCode)
    {
        var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(c => c.CouponCode.ToLower() == couponCode.ToLower());

        var res = _mapper.Map<CouponDto>(couponFromDb);

        return new ResponseDto
        {
            Result = res,
            IsSuccess = couponFromDb != null,
            Message = couponFromDb == null ? "Coupon Not Found" : "Coupon Found"
        };
    }
    
    [HttpPost]
    public async Task<ResponseDto> CreateCouponAsync([FromBody] CouponDto couponDto)
    {
        var coupon = _mapper.Map<Coupon>(couponDto);
        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync();
        couponDto.Id = coupon.Id;
        
        return new ResponseDto
        {
            Result = couponDto,
            IsSuccess = true,
            Message = "Coupon Created Successfully"
        };
    }
    
    [HttpDelete("{id:int}")]
    public async Task<ResponseDto> DeleteCouponAsync(int id)
    {
        try
        {
            var couponFromDb = await _db.Coupons.FirstOrDefaultAsync(c => c.Id == id);
            if (couponFromDb == null)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Coupon Not Found",
                };
            }

            _db.Coupons.Remove(couponFromDb);
            await _db.SaveChangesAsync();

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Coupon Deleted Successfully",
            };
        }
        catch (Exception e)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = e.Message,
            };
        }
    }
}