using AutoMapper;
using Mango.Services.ShoppingCart.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using MessageBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Services;

namespace ShoppingCartAPI.Controllers;

[ApiController]
[Route("api/Cart")]
public class ShoppingCartController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly ResponseDto _response;
    private readonly IProductService _productService;
    private readonly ICouponService _couponService;
    private readonly IMessageProducer _messageProducer;

    public ShoppingCartController(AppDbContext context, IMapper mapper, ResponseDto response,
        IProductService productService, ICouponService couponService, IMessageProducer messageProducer,
        IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _response = response;
        _productService = productService;
        _couponService = couponService;
        _messageProducer = messageProducer;
        _configuration = configuration;
    }

    [HttpGet("GetCart/{userId}")]
    public async Task<ResponseDto> GetCart(string userId)
    {
        try
        {
            var cart = new CartDto
            {
                CartHeader =
                    _mapper.Map<CartHeaderDto>(await _context.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId)),
            };
            cart.CartDetails =
                _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(u =>
                    u.CartHeader.UserId == userId));

            IEnumerable<ProductDto> productDtos = await _productService.GetProducts();

            foreach (var item in cart.CartDetails)
            {
                item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                cart.CartHeader.CartTotal += item.Product.Price * item.Count;
            }

            if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                {
                    cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                    cart.CartHeader.Discount = coupon.DiscountAmount;
                }
            }

            _response.IsSuccess = true;
            _response.Result = cart;
        }
        catch (Exception e)
        {
            _response.Message = e.Message;
            _response.IsSuccess = false;
        }

        return _response;
    }

    [HttpPost("ApplyCoupon")]
    public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartFromDb =
                _context.CartHeaders.FirstOrDefault(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId);

            if (cartFromDb == null)
            {
                return BadRequest();
            }

            cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
            _context.CartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }

    [HttpPost("EmailCartRequest")]
    public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
    {
        try
        {
            await _messageProducer.PublishMessage(cartDto,
                _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.ToString();
        }

        return _response;
    }

    [HttpPost("RemoveCoupon")]
    public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
    {
        try
        {
            var cartFromDb =
                _context.CartHeaders.FirstOrDefault(u => u.CartHeaderId == cartDto.CartHeader.CartHeaderId);

            if (cartFromDb == null)
            {
                return BadRequest();
            }

            cartFromDb.CouponCode = "";
            _context.CartHeaders.Update(cartFromDb);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }

    [HttpPost("CartUpsert")]
    public async Task<ResponseDto> CartUpsert(CartDto cartDto)
    {
        try
        {
            var cartHeaderFromDb =
                await _context.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

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

    [HttpPost("RemoveCart")]
    public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
    {
        try
        {
            var cartDetails = _context.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

            int totalCountOfCartItems =
                _context.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
            _context.CartDetails.Remove(cartDetails);
            if (totalCountOfCartItems == 1)
            {
                var cartHeaderToRemove =
                    await _context.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                _context.CartHeaders.Remove(cartHeaderToRemove);
            }

            await _context.SaveChangesAsync();
            _response.Result = true;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }
}