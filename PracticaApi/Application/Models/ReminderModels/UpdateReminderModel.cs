﻿using Domain.Containers;
using Domain.Reminders;

namespace Application.Models.ReminderModels;
public class UpdateReminderModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
    public Guid ModifiedBy { get; set; }
}