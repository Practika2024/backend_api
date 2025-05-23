using Api.Dtos.ContainersType;
using AutoMapper;
using DataAccessLayer.Entities.Containers;
using Domain.Common.Models;
using Domain.ContainerTypes;
using Domain.ContainerTypes.Models;

namespace Api.MappingProfiles;

public class ContainerTypeMapperProfile : Profile
{
    public ContainerTypeMapperProfile()
    {
        CreateMap<ContainerTypeDto, ContainerType>().ReverseMap();
        CreateMap<EntitiesListModel<ContainerTypeDto>, EntitiesListModel<ContainerType>>().ReverseMap();
        CreateMap<CreateContainerTypeModel, ContainerTypeEntity>().ReverseMap();
    }
}