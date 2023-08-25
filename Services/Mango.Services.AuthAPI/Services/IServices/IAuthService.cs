using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IServices;

public interface IAuthService
{
    Task<ResponseDto> RegisterUserAsync(RegistrationRequestDto userDto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto);
    Task<bool> AssignRoleAsync(string email, string roleName);
}