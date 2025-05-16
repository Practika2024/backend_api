using Api.Dtos.ReminderType;
using AutoMapper;
using DataAccessLayer.Entities.Reminders;
using Domain.Common.Models;
using Domain.ReminderTypes;
using Domain.ReminderTypes.Models;

namespace Api.MappingProfiles;

public class ReminderTypeMapperProfile : Profile
{
    public ReminderTypeMapperProfile()
    {
        CreateMap<ReminderTypeDto, ReminderType>().ReverseMap();
        CreateMap<EntitiesListModel<ReminderTypeDto>, EntitiesListModel<ReminderType>>().ReverseMap();
        CreateMap<CreateReminderTypeModel, ReminderTypeEntity>().ReverseMap();
    }
}