using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.Containers;
using Domain.Containers.Models;

namespace DataAccessLayer.MappingProfiles;

public class ContainersMapperProfile : Profile
{
    public ContainersMapperProfile()
    {
        CreateMap<Container, CreateContainerModel>().ReverseMap();
        
        CreateMap<ContainerEntity, Container>().ReverseMap();
        CreateMap<ContainerEntity, CreateContainerModel>().ReverseMap();
        CreateMap<ContainerEntity, SetContainerContentModel>().ReverseMap();
        CreateMap<SetContainerContentModel, ContainerContentEntity>().ReverseMap();
    }
}