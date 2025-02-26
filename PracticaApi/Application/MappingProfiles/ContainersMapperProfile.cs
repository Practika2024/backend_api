using AutoMapper;
using Domain.Containers;
using Domain.Containers.Models;

namespace Application.MappingProfiles;

public class ContainersMapperProfile : Profile
{
    public ContainersMapperProfile()
    {
        CreateMap<Container, UpdateContainerModel>().ReverseMap();
    }
}