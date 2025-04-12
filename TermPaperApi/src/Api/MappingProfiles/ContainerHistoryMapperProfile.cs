using Api.Dtos.ContainerHistories;
using AutoMapper;
using Domain.ContainersHistory;

namespace Api.MappingProfiles;

public class ContainerHistoryMapperProfile : Profile
{
    public ContainerHistoryMapperProfile()
    {
        CreateMap<ContainerHistory, ContainerHistoryDto>().ReverseMap();
    }
}