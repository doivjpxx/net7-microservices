using Mango.Web.Models;

namespace Mango.Web.Services.IServices;

public interface IAuthService
{
    Task<ResponseDto?> LoginAsync(LoginRequestDto request);
    Task<ResponseDto?> RegisterUserAsync(RegistrationRequestDto request);
    Task<ResponseDto?> AssignRoleAsync(AssignRoleDto request);
}