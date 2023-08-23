using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface ICouponService
{
    Task<ResponseDto?> GetCouponsAsync();
    Task<ResponseDto?> GetCouponAsync(int id);
    Task<ResponseDto?> CreateEditCouponAsync(CouponDto couponDto);
    Task<ResponseDto?> DeleteCouponAsync(int id);
}