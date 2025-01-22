using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dto.Upsert;

namespace Mango.Services.ShoppingCartAPI;

public class MappingConfig: Profile
{
    public MappingConfig()
    {
        CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
        CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
        CreateMap<Product, ProductDto>().ReverseMap();
        
        // for upsert
        CreateMap<UpsertCartHeaderDto, CartHeader>();
        CreateMap<UpsertCartDetailsDto, CartDetails>();
    }
}

