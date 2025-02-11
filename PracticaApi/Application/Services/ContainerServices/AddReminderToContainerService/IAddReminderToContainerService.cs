using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Containers;
using Domain.Reminders;

namespace Application.Services.ContainerServices.AddReminderToContainerService;
public interface IAddReminderToContainerService
{
    Task<Result<ReminderVM, ContainerException>> AddReminderAsync(
        Guid containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        CancellationToken cancellationToken);
}