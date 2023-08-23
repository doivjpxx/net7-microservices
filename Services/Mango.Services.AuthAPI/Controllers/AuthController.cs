using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Auth API");
    }
    
    [HttpPost]
    public IActionResult Post()
    {
        return Ok("Auth API");
    }
}