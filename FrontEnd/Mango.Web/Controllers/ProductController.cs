using Mango.Web.Models;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var products = new List<ProductDto>();
        
        var res = await _productService.GetAllProductsAsync();

        if (res.IsSuccess)
        {
            products = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(res.Result));
        }
        else
        {
            TempData["error"] = res.Message;
        }

        return View(products);
    }
}