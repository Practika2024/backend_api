using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.ContainerModels;
using Domain.ContainerTypeModels;

namespace DataAccessLayer.MappingProfiles;

public class ContainersTypeMapperProfile : Profile
{
    public ContainersTypeMapperProfile()
    {
        CreateMap<ContainerTypeEntity, ContainerType>().ReverseMap();
        CreateMap<UpdateContainerTypeModel, ContainerTypeEntity>().ReverseMap();
    }
}