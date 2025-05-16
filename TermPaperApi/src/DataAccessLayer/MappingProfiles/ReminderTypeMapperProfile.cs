using AutoMapper;
using DataAccessLayer.Entities.Reminders;
using Domain.ReminderTypes.Models;
using ReminderType = Domain.ReminderTypes.ReminderType;

namespace DataAccessLayer.MappingProfiles;

public class ReminderTypeMapperProfile : Profile
{
    public ReminderTypeMapperProfile()
    {
        CreateMap<ReminderTypeEntity, ReminderType>().ReverseMap();
        CreateMap<UpdateReminderTypeModel, ReminderTypeEntity>().ReverseMap();
    }
}