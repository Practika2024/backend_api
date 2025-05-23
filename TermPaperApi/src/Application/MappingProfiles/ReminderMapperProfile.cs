using AutoMapper;
using Domain.Reminders;
using Domain.Reminders.Models;

namespace Application.MappingProfiles;

public class ReminderMapperProfile : Profile
{
    public ReminderMapperProfile()
    {
        CreateMap<Reminder, CreateReminderModel>().ReverseMap();
        CreateMap<Reminder, UpdateReminderModel>().ReverseMap();
        CreateMap<Reminder, DeleteReminderModel>().ReverseMap();
    }
}