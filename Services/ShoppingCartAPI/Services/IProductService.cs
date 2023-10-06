using Mango.Services.ShoppingCart.Models.Dtos;

namespace ShoppingCartAPI.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProducts();
}