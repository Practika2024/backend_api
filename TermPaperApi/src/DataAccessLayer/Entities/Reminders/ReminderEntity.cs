﻿using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;
using Domain.Reminders;

namespace DataAccessLayer.Entities.Reminders;
public class ReminderEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public ContainerEntity? Container { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public int TypeId { get; set; }
    public ReminderTypeEntity? Type { get; set; }
    public string? HangfireJobId { get; set; }
    public bool IsViewed { get; set; }
}