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
    
    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto model)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? res = await _productService.CreateProductsAsync(model);

            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Product created";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = "Product creation failed";
            }
        }

        return View(model);
    }

    public async Task<IActionResult> Delete(int productId)
    {
        ResponseDto? response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {
            ProductDto? model= JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(ProductDto model)
    {
        var res = await _productService.DeleteProductsAsync(model.ProductId);

        if (res.IsSuccess)
        {
            TempData["success"] = "Product deleted";
            return RedirectToAction(nameof(Index));
        }
        else
        {
            TempData["error"] = res.Message;
        }

        return View(model);
    }

    public async Task<IActionResult> Edit(int productId)
    {
        ResponseDto? response = await _productService.GetProductByIdAsync(productId);

        if (response != null && response.IsSuccess)
        {
            ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(ProductDto productDto)
    {
        if (ModelState.IsValid)
        {
            ResponseDto? response = await _productService.UpdateProductsAsync(productDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }
        return View(productDto);
    }
}