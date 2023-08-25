using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        var res = await _authService.LoginAsync(model);
        if (res.User == null)
        {
            return BadRequest();
        }

        return Ok(res);
    }
    
    [HttpPost("register")]
    public async Task<ResponseDto> Register([FromBody] RegistrationRequestDto model)
    {
        if (ModelState.IsValid)
        {
            var result = await _authService.RegisterUserAsync(model);
            return result.IsSuccess ? new ResponseDto { IsSuccess = true } : new ResponseDto { IsSuccess = false, Message = result.Message };
        }

        return new ResponseDto { IsSuccess = false, Message = "Some data annotation errors" };
    }
}