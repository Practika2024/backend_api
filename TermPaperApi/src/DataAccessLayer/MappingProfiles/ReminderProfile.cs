using AutoMapper;
using DataAccessLayer.Entities.Reminders;
using Domain.Reminders;
using Domain.Reminders.Models;

namespace DataAccessLayer.MappingProfiles;

public class ReminderProfile : Profile
{
    public ReminderProfile()
    {
        CreateMap<CreateReminderModel, ReminderEntity>().ReverseMap();
        CreateMap<UpdateReminderModel, ReminderEntity>().ReverseMap();
        CreateMap<ReminderEntity, Reminder>().ReverseMap();
    }
}