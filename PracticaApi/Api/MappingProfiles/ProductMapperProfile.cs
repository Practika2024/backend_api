using Api.Dtos.Products;
using AutoMapper;
using Domain.ProductModels;

namespace Api.MappingProfiles;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
        CreateMap<CreateProductDto, Product>().ReverseMap();
    }
}