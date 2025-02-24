using AutoMapper;
using Domain.ContainerModels;

namespace Application.MappingProfiles;

public class ContainersMapperProfile : Profile
{
    public ContainersMapperProfile()
    {
        CreateMap<Container, UpdateContainerModel>().ReverseMap();
    }
}