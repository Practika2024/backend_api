using AutoMapper;
using DataAccessLayer.Entities.ContainerHistories;
using Domain.ContainersHistory;
using Domain.ContainersHistory.Models;

namespace DataAccessLayer.MappingProfiles;

public class ContainersHistoryMapperProfile : Profile
{
    public ContainersHistoryMapperProfile()
    {
        CreateMap<ContainerHistoryEntity, CreateContainerHistoryModel>().ReverseMap();
        CreateMap<ContainerHistoryEntity, ContainerHistory>().ReverseMap();
        CreateMap<CreateContainerHistoryModel, ContainerHistory>().ReverseMap();
    }
}