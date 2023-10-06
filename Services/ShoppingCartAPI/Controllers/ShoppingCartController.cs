using AutoMapper;
using Mango.Services.ShoppingCart.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;

namespace ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/cart")]
public class ShoppingCartController : ControllerBase
{
    private IMapper _mapper;
    private readonly AppDbContext _context;
    private ResponseDto _response;

    public ShoppingCartController(AppDbContext context, IMapper mapper, ResponseDto response)
    {
        _context = context;
        _mapper = mapper;
        _response = response;
    }

    [HttpPost("CartUpsert")]
    public async Task<ResponseDto> CartUpsert(CartDto cartDto)
    {
        try
        {
            var cartHeaderFromDb =
                await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

            if (cartHeaderFromDb == null)
            {
                // create header and details
                var cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                _context.CartHeaders.Add(cartHeader);

                cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails));
                await _context.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _context.CartDetails.AsNoTracking().FirstOrDefaultAsync(u =>
                    u.ProductId == cartDto.CartDetails.First().ProductId &&
                    u.CartHeaderId == cartHeaderFromDb.CartHeaderId);

                if (cartDetailsFromDb == null)
                {
                    // create cartdetails
                    cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    _context.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // update count in cart details
                    cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                    cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                    cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                    _context.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
            }

            _response.IsSuccess = true;
            _response.Result = cartDto;
        }
        catch (Exception e)
        {
            _response.Message = e.Message.ToString();
            _response.IsSuccess = false;
        }

        return _response;
    }
}