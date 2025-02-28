using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Reminders;
using Domain.Reminders.Models;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record DeleteReminderCommand : IRequest<Result<Reminder, ReminderException>>
{
    public required Guid Id { get; init; }
}

public class DeleteReminderCommandHandler(
    IReminderRepository reminderRepository) : IRequestHandler<DeleteReminderCommand, Result<Reminder, ReminderException>>
{
    public async Task<Result<Reminder, ReminderException>> Handle(
        DeleteReminderCommand request,
        CancellationToken cancellationToken)
    {
        var reminderId = request.Id;
        var existingReminder = await reminderRepository.GetById(reminderId, cancellationToken);

        return await existingReminder.Match(
            async reminder =>
            {
                try
                {
                    var deleteModel = new DeleteReminderModel
                    {
                        Id = reminderId
                    };

                    var deletedReminder = await reminderRepository.Delete(deleteModel, cancellationToken);
                    return deletedReminder;
                }
                catch (ReminderException exception)
                {
                    return new ReminderUnknownException(reminderId, exception);
                }
            },
            () => Task.FromResult<Result<Reminder, ReminderException>>(
                new ReminderNotFoundException(reminderId))
        );
    }
}