using Api.Dtos.Products;
using AutoMapper;
using Domain.Common.Models;
using Domain.Products;

namespace Api.MappingProfiles;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
        CreateMap<EntitiesListModel<ProductDto>, EntitiesListModel<Product>>().ReverseMap();
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();
        CreateMap<ProductImageDto, ProductImage>().ReverseMap();
    }
}