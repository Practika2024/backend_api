using AutoMapper;
using DataAccessLayer.Entities.Reminders;
using Domain.Reminders;
using Domain.Reminders.Models;

namespace DataAccessLayer.MappingProfiles;

public class ReminderProfile : Profile
{
    public ReminderProfile()
    {
        CreateMap<CreateReminderModel, ReminderEntity>();
        CreateMap<ReminderEntity, Reminder>();
        CreateMap<Reminder, UpdateReminderModel>()
            .ForMember(dest => dest.Title, opt => opt.Condition(src => src.Title != null))
            .ForMember(dest => dest.DueDate, opt => opt.Condition(src => src.DueDate != null))
            .ForMember(dest => dest.Type, opt => opt.Condition(src => src.Type != null));
    }
}