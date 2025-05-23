using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.ContainerTypes;
using Domain.ContainerTypes.Models;

namespace DataAccessLayer.MappingProfiles;

public class ContainersTypeMapperProfile : Profile
{
    public ContainersTypeMapperProfile()
    {
        CreateMap<ContainerTypeEntity, ContainerType>().ReverseMap();
        CreateMap<UpdateContainerTypeModel, ContainerTypeEntity>().ReverseMap();
    }
}