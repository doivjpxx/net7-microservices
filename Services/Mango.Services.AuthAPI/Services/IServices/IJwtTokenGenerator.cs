using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;

namespace Mango.Services.AuthAPI.Services.IServices;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
}