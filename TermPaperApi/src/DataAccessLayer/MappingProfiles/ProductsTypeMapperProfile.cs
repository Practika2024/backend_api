using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.ProductTypes.Models;
using ProductType = Domain.ProductTypes.ProductType;

namespace DataAccessLayer.MappingProfiles;

public class ProductsTypeMapperProfile : Profile
{
    public ProductsTypeMapperProfile()
    {
        CreateMap<ProductTypeEntity, ProductType>().ReverseMap();
        CreateMap<UpdateProductTypeModel, ProductTypeEntity>().ReverseMap();
    }
}