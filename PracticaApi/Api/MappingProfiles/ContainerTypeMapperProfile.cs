using Api.Dtos.ContainersType;
using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.ContainerTypeModels;

namespace Api.MappingProfiles;

public class ContainerTypeMapperProfile : Profile
{
    public ContainerTypeMapperProfile()
    {
        CreateMap<ContainerTypeDto, ContainerType>().ReverseMap();
        CreateMap<CreateContainerTypeModel, ContainerTypeEntity>().ReverseMap();
    }
}