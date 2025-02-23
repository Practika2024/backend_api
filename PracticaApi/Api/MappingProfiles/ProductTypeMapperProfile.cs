using Api.Dtos.ProductsType;
using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.ProductTypeModels;
using ProductType = Domain.ProductTypeModels.ProductType;

namespace Api.MappingProfiles;

public class ProductTypeMapperProfile : Profile
{
    public ProductTypeMapperProfile()
    {
        CreateMap<ProductTypeDto, ProductType>().ReverseMap();
        CreateMap<CreateProductTypeModel, ProductTypeEntity>().ReverseMap();
    }
}