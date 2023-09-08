using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dtos;

namespace Mango.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        MapperConfiguration mappingConfig = new(config =>
        {
            config.CreateMap<Product, ProductDto>().ReverseMap();
            config.CreateMap<ProductDto, ResponseDto>().ReverseMap();
        });

        return mappingConfig;
    }
}