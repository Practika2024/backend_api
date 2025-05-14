using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ReminderService;
using Domain.Reminders;
using Domain.Reminders.Models;
using Hangfire;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record DeleteReminderCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteReminderCommandHandler(
    IReminderRepository reminderRepository,
    IUserProvider userProvider,
    IReminderService reminderService) : IRequestHandler<DeleteReminderCommand, ServiceResponse>
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
                    if (userProvider.GetUserId() != reminder.CreatedBy)
                    {
                        return ServiceResponse.ForbiddenResponse("You are not allowed to delete this reminder");
                    }
                    
                    if (!string.IsNullOrWhiteSpace(reminder.HangfireJobId))
                    {
                        // BackgroundJob.Delete(reminder.HangfireJobId);
                        reminderService.DeleteHangfireJob(reminder.HangfireJobId);
                    }

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
