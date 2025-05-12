using Api.Dtos.ProductsType;
using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.Common.Models;
using Domain.ProductTypes.Models;
using ProductType = Domain.ProductTypes.ProductType;

namespace Api.MappingProfiles;

public class ProductTypeMapperProfile : Profile
{
    public ProductTypeMapperProfile()
    {
        CreateMap<ProductTypeDto, ProductType>().ReverseMap();
        CreateMap<EntitiesListModel<ProductTypeDto>, EntitiesListModel<ProductType>>().ReverseMap();
        CreateMap<CreateProductTypeModel, ProductTypeEntity>().ReverseMap();
    }
}