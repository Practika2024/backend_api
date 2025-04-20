using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services.ReminderService;
using Domain.Reminders;
using Domain.Reminders.Models;
using MediatR;
using Optional.Unsafe;

namespace Application.Commands.Reminders.Commands;

public record AddReminderToContainerCommand : IRequest<Result<Reminder, ReminderException>>
{
    public required Guid ContainerId { get; init; }
    public required string Title { get; init; } = null!;
    public required DateTime DueDate { get; init; }
    public required ReminderType Type { get; init; }
}

public class AddReminderToContainerCommandHandler(
    IContainerRepository containerRepository,
    IReminderRepository reminderRepository,
    IUserProvider userProvider,
    IReminderService reminderService,
    IUserQueries userQueries)
    : IRequestHandler<AddReminderToContainerCommand, Result<Reminder, ReminderException>>
{
    public async Task<Result<Reminder, ReminderException>> Handle(
        AddReminderToContainerCommand request,
        CancellationToken cancellationToken)
    {
        var containerId = request.ContainerId;
        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);
        var reminderId = Guid.NewGuid();

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var userId = userProvider.GetUserId();
                    var createReminderModel = new CreateReminderModel
                    {
                        Id = reminderId,
                        ContainerId = containerId,
                        Title = request.Title,
                        DueDate = request.DueDate,
                        Type = request.Type,
                        CreatedBy = userId
                    };

                    var createdReminder = await reminderRepository.Create(createReminderModel, cancellationToken);

                    if ((await userQueries.GetById(userId, cancellationToken)).ValueOrDefault().EmailConfirmed)
                    {
                        // Плануємо нагадування через Hangfire
                        // var reminderTime = createdReminder.DueDate.AddMinutes(-30);
                        var reminderTime = createdReminder.DueDate.AddSeconds(-30);
                        reminderService.ScheduleReminder(await userQueries.GetEmailByUserId(userId, cancellationToken),
                            createdReminder.Title, reminderTime);
                    }

                    return createdReminder;
                }
                catch (ReminderException exception)
                {
                    return new ReminderUnknownException(reminderId, exception);
                }
            },
            () => Task.FromResult<Result<Reminder, ReminderException>>(
                new ContainerForReminderNotFoundException(reminderId))
        );
    }
}