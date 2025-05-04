using Api.Dtos.ProductsType;
using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.ProductTypes.Models;
using ProductType = Domain.ProductTypes.ProductType;

namespace Api.MappingProfiles;

public class ProductTypeMapperProfile : Profile
{
    public ProductTypeMapperProfile()
    {
        CreateMap<ProductTypeDto, ProductType>().ReverseMap();
        CreateMap<CreateProductTypeModel, ProductTypeEntity>().ReverseMap();
    }
}