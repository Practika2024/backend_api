using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Reminders;
using Optional;

namespace Application.Services.ContainerServices.AddReminderToContainerService;
public class AddReminderToContainerService : IAddReminderToContainerService
{
    private readonly IContainerRepository _containerRepository;
    private readonly IReminderRepository _reminderRepository;

    public AddReminderToContainerService(
        IContainerRepository containerRepository,
        IReminderRepository reminderRepository)
    {
        _containerRepository = containerRepository;
        _reminderRepository = reminderRepository;
    }

    public async Task<Result<ReminderVM, ContainerException>> AddReminderAsync(
        Guid containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        CancellationToken cancellationToken)
    {
        var containerIdObj = new ContainerId(containerId);
        var existingContainer = await _containerRepository.GetById(containerIdObj, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var reminderId = ReminderId.New();
                    var reminder = ReminderEntity.New(reminderId, containerIdObj, title, dueDate, type, UserId.Empty);

                    var createdReminder = await _reminderRepository.Create(reminder, cancellationToken);
                    return new ReminderVM(createdReminder);
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerIdObj, exception);
                }
            },
            () => Task.FromResult<Result<ReminderVM, ContainerException>>(new ContainerNotFoundException(containerIdObj))
        );
    }
}