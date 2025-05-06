using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Reminders;
using Domain.Reminders.Models;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record DeleteReminderCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteReminderCommandHandler(
    IReminderRepository reminderRepository) : IRequestHandler<DeleteReminderCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
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
                    return ServiceResponse.OkResponse("Reminder deleted", deletedReminder);
                }
                catch (ReminderException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Reminder not found"))
        );
    }
}