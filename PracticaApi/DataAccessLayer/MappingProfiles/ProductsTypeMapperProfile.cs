using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.ProductTypeModels;
using ProductType = Domain.ProductTypeModels.ProductType;

namespace DataAccessLayer.MappingProfiles;

public class ProductsTypeMapperProfile : Profile
{
    public ProductsTypeMapperProfile()
    {
        CreateMap<ProductTypeEntity, ProductType>().ReverseMap();
        CreateMap<UpdateProductTypeModel, ProductTypeEntity>().ReverseMap();
    }
}