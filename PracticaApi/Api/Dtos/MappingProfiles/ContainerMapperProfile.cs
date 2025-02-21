using Api.Dtos.Containers;
using AutoMapper;
using Domain.Containers;

namespace Api.Dtos.MappingProfiles;

public class ContainerMapperProfile : Profile
{
    public ContainerMapperProfile()
    {
        CreateMap<ContainerDto, ContainerEntity>().ReverseMap();
        CreateMap<CreateContainerDto, ContainerEntity>().ReverseMap();
    }
}