using Api.Dtos.ContainerHistories;
using AutoMapper;
using Domain.Common.Models;
using Domain.ContainersHistory;

namespace Api.MappingProfiles;

public class ContainerHistoryMapperProfile : Profile
{
    public ContainerHistoryMapperProfile()
    {
        CreateMap<ContainerHistory, ContainerHistoryDto>().ReverseMap();
        CreateMap<EntitiesListModel<ContainerHistoryDto>, EntitiesListModel<ContainerHistory>>().ReverseMap();
    }
}