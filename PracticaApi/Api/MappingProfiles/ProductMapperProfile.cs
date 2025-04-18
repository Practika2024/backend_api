﻿using Api.Dtos.Products;
using AutoMapper;
using Domain.Products;

namespace Api.MappingProfiles;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        CreateMap<ProductDto, Product>().ReverseMap();
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();
    }
}