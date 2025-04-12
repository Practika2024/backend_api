using AutoMapper;
using Domain.Products;
using Domain.Products.Models;

namespace Application.MappingProfiles;

public class ProductsMapperProfile : Profile
{
    public ProductsMapperProfile()
    {
        CreateMap<Product, UpdateProductModel>().ReverseMap();
    }
}