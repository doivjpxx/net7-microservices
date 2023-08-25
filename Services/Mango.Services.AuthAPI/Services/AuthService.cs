using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.AuthAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ResponseDto> RegisterUserAsync(RegistrationRequestDto userDto)
    {
        var user = new ApplicationUser()
        {
            Email = userDto.Email,
            UserName = userDto.Email,
            NormalizedEmail = userDto.Email.ToUpper(),
            Name = userDto.Name,
            PhoneNumber = userDto.PhoneNumber,
        };

        try
        {
            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                var createdUser = _db.ApplicationUsers.First(x => x.UserName == userDto.Email);

                var returnUser = new UserDto
                {
                    Name = createdUser.Name,
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    PhoneNumber = createdUser.PhoneNumber
                };

                return new ResponseDto { IsSuccess = true, Result = returnUser };
            }
            else
            {
                return new ResponseDto { IsSuccess = false, Message = result.Errors.FirstOrDefault().Description };
            }
        }
        catch (Exception e)
        {
            return new ResponseDto { IsSuccess = false, Message = e.Message };
        }
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x =>
            x.UserName.ToLower() == loginDto.Email.ToLower());

        bool isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (isValid == false)
        {
            return new LoginResponseDto() { User = null, Token = "" };
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        LoginResponseDto response = new LoginResponseDto
        {
            Token = token,
            User = new UserDto { Name = user.Name, Email = user.Email, Id = user.Id, PhoneNumber = user.PhoneNumber },
        };

        return response;
    }

    public async Task<bool> AssignRoleAsync(string email, string roleName)
    {
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());
        if (user != null)
        {
            if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
            {
                // create role if it doesn't exist
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                
              
            }
            // assign role to user
            await _userManager.AddToRoleAsync(user, roleName);
            
            return true;
        }

        return false;
    }
}