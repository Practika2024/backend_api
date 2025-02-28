using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Reminders;
using Domain.Reminders.Models;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record UpdateReminderCommand : IRequest<Result<Reminder, ReminderException>>
{
    public required Guid Id { get; init; }
    public string? Title { get; init; }
    public DateTime? DueDate { get; init; }
    public ReminderType? Type { get; init; }
}

public class UpdateReminderCommandHandler(
    IReminderRepository reminderRepository) : IRequestHandler<UpdateReminderCommand, Result<Reminder, ReminderException>>
{
    public async Task<Result<Reminder, ReminderException>> Handle(
        UpdateReminderCommand request,
        CancellationToken cancellationToken)
    {
        var reminderId = request.Id;
        var existingReminder = await reminderRepository.GetById(reminderId, cancellationToken);

        return await existingReminder.Match(
            async reminder =>
            {
                try
                {
                    var updateModel = new UpdateReminderModel
                    {
                        Id = reminderId,
                        Title = request.Title ?? reminder.Title,
                        DueDate = request.DueDate ?? reminder.DueDate,
                        Type = request.Type ?? reminder.Type
                    };

                    var updatedReminder = await reminderRepository.Update(updateModel, cancellationToken);
                    return updatedReminder;
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