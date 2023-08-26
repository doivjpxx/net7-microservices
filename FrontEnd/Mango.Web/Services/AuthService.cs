using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Mango.Web.Utils;

namespace Mango.Web.Services;

public class AuthService : IAuthService
{
    private readonly IBaseService _baseService;

    public AuthService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var res = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = request,
            Url = SD.AuthAPIBase + "/login"
        });

        return res;
    }

    public async Task<ResponseDto?> RegisterUserAsync(RegistrationRequestDto request)
    {
        var res = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = request,
            Url = SD.AuthAPIBase + "/register"
        });

        return res;
    }

    public async Task<ResponseDto?> AssignRoleAsync(AssignRoleDto request)
    {
        var res = await _baseService.SendAsync(new RequestDto
        {
            ApiType = SD.ApiType.POST,
            Data = request,
            Url = SD.AuthAPIBase + "/assign-role"
        });

        return res;
    }
}