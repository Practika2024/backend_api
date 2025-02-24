using Api.Dtos.Containers;
using AutoMapper;
using Domain.ContainerModels;

namespace Api.MappingProfiles;

public class ContainerMapperProfile : Profile
{
    public ContainerMapperProfile()
    {
        CreateMap<ContainerDto, Container>().ReverseMap();
        CreateMap<CreateContainerDto, Container>().ReverseMap();
        
        CreateMap<ContainerWithContentDto, Container>().ReverseMap();
        CreateMap<ContentDto, ContainerContent>().ReverseMap();
    }
}