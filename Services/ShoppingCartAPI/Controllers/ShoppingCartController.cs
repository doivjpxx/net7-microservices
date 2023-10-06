using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartAPI.Data;

namespace ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class ShoppingCartController : ControllerBase
{
    private IMapper _mapper;
    private readonly AppDbContext _context;

    public ShoppingCartController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
}