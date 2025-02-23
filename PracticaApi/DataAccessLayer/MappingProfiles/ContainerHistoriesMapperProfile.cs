using AutoMapper;
using DataAccessLayer.Entities.ContainerHistories;
using Domain.ContainerHistoryModels;
using Domain.ContainerModels;

namespace DataAccessLayer.MappingProfiles;

public class ContainerHistoriesMapperProfile: Profile
{
    public ContainerHistoriesMapperProfile()
    {
        CreateMap<ContainerHistory, ContainerHistoryEntity>().ReverseMap();
    }
}


