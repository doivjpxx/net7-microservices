using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly AppDbContext _context;
    private ResponseDto _response;
    private IMapper _mapper;

    public ProductController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _response = new ResponseDto();
    }

    [HttpGet]
    public ResponseDto Get()
    {
        try
        {
            IEnumerable<Product> productList = _context.Products.ToList();
            _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productList);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }

    [HttpGet]
    [Route("{id:int}")]
    public ResponseDto Get(int id)
    {
        try
        {
            Product obj = _context.Products.First(u => u.ProductId == id);
            _response.Result = _mapper.Map<ProductDto>(obj);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Post(ProductDto productDto)
    {
        try
        {
            Product product = _mapper.Map<Product>(productDto);
            product.ImageUrl = "https://placehold.co/600x400";
            _context.Products.Add(product);
            _context.SaveChanges();

            // if (productDto.Image != null)
            // {
            //     string fileName = product.ProductId + Path.GetExtension(productDto.Image.FileName);
            //     string filePath = @"wwwroot\ProductImages\" + fileName;
            //
            //     //I have added the if condition to remove the any image with same name if that exist in the folder by any change
            //     var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            //     FileInfo file = new FileInfo(directoryLocation);
            //     if (file.Exists)
            //     {
            //         file.Delete();
            //     }
            //
            //     var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            //     using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
            //     {
            //         productDto.Image.CopyTo(fileStream);
            //     }
            //
            //     var baseUrl =
            //         $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            //     product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
            //     product.ImageLocalPath = filePath;
            // }
            // else
            // {
            //     product.ImageUrl = "https://placehold.co/600x400";
            // }

            // _context.Products.Update(product);
            // _context.SaveChanges();
            _response.Result = _mapper.Map<ProductDto>(product);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.Message = e.Message;
        }

        return _response;
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Put(ProductDto ProductDto)
    {
        try
        {
            Product product = _mapper.Map<Product>(ProductDto);

            // if (ProductDto.Image != null)
            // {
            //     if (!string.IsNullOrEmpty(product.ImageLocalPath))
            //     {
            //         var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
            //         FileInfo file = new FileInfo(oldFilePathDirectory);
            //         if (file.Exists)
            //         {
            //             file.Delete();
            //         }
            //     }
            //
            //     string fileName = product.ProductId + Path.GetExtension(ProductDto.Image.FileName);
            //     string filePath = @"wwwroot\ProductImages\" + fileName;
            //     var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            //     using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
            //     {
            //         ProductDto.Image.CopyTo(fileStream);
            //     }
            //
            //     var baseUrl =
            //         $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            //     product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
            //     product.ImageLocalPath = filePath;
            // }


            _context.Products.Update(product);
            _context.SaveChanges();

            _response.Result = _mapper.Map<ProductDto>(product);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public ResponseDto Delete(int id)
    {
        try
        {
            Product obj = _context.Products.First(u => u.ProductId == id);
            // if (!string.IsNullOrEmpty(obj.ImageLocalPath))
            // {
            //     var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
            //     FileInfo file = new FileInfo(oldFilePathDirectory);
            //     if (file.Exists)
            //     {
            //         file.Delete();
            //     }
            // }

            _context.Products.Remove(obj);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return _response;
    }
}