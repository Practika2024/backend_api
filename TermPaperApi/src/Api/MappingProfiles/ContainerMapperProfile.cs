using Api.Dtos.Containers;
using AutoMapper;
using Domain.Containers;

namespace Api.MappingProfiles;

public class ContainerMapperProfile : Profile
{
    public ContainerMapperProfile()
    {
        CreateMap<ContainerDto, Container>().ReverseMap();
        CreateMap<CreateContainerDto, Container>().ReverseMap();
    }
}