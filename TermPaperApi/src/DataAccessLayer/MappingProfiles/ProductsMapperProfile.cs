using AutoMapper;
using DataAccessLayer.Entities.Products;
using Domain.Products;
using Domain.Products.Models;

namespace DataAccessLayer.MappingProfiles;

public class ProductsMapperProfile : Profile
{
    public ProductsMapperProfile()
    {
        CreateMap<Product, CreateProductModel>().ReverseMap();
        
        CreateMap<ProductEntity, Product>().ReverseMap();
        CreateMap<ProductEntity, UpdateProductModel>().ReverseMap();
        
        CreateMap<ProductImage, ProductImageEntity>().ReverseMap();
    }
}