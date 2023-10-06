using Mango.Services.ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Models;
using AutoMapper;

namespace ShoppingCartAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
            config.CreateMap<CartDetailsDto, CartDetails>().ReverseMap();
        });

        return mappingConfig;
    }
}