﻿using Domain.Reminders;

namespace Application.Dtos.Reminders;

public class AddReminderToContainerDto
{
    public string Title { get; set; } = null!; 
    public DateTime DueDate { get; set; } 
    public ReminderType Type { get; set; } 
    public Guid CreatedBy { get; set; } 
}