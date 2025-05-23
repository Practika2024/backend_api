﻿namespace Domain.Reminders.Models;
public class CreateReminderModel
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public int TypeId { get; set; }
    public Guid CreatedBy { get; set; }
    public string? HangfireJobId { get; set; }
}