using Api.Dtos.Containers;
using AutoMapper;
using Domain.Common.Models;
using Domain.Containers;

namespace Api.MappingProfiles;

public class ContainerMapperProfile : Profile
{
    public ContainerMapperProfile()
    {
        CreateMap<ContainerDto, Container>().ReverseMap();
        CreateMap<EntitiesListModel<ContainerDto>, EntitiesListModel<Container>>().ReverseMap();
        CreateMap<CreateContainerDto, Container>().ReverseMap();
    }
}