﻿using Api.Dtos.Reminders;
using AutoMapper;
using DataAccessLayer.Entities.Reminders;
using Domain.Reminders;
using Domain.Reminders.Models;

namespace Api.MappingProfiles;

public class ReminderProfile : Profile
{
    public ReminderProfile()
    {
        CreateMap<Reminder, ReminderDto>();
        CreateMap<UpdateReminderDto, UpdateReminderModel>();
        CreateMap<DeleteReminderDto, DeleteReminderModel>();
    }
}