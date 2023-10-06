using Mango.Services.ShoppingCart.Models.Dtos;
using Newtonsoft.Json;

namespace ShoppingCartAPI.Services;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<ProductDto>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("Product");
        var response = await client.GetAsync($"/api/Product");
        var apiContent = await response.Content.ReadAsStringAsync();
        var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

        if (res.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(res.Result));
        }
        
        return new List<ProductDto>();
    }
}