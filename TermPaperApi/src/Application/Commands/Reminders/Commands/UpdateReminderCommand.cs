using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ReminderService;
using Domain.Reminders.Models;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record UpdateReminderCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
    public string? Title { get; init; }
    public DateTime? DueDate { get; init; }
    public int? TypeId { get; init; }
    public Guid? ContainerId { get; init; }
}

public class UpdateReminderCommandHandler(
    IReminderRepository reminderRepository,
    IReminderService reminderService,
    IUserProvider userProvider,
    IUserQueries userQueries
) : IRequestHandler<UpdateReminderCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
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
                    var newTitle = request.Title ?? reminder.Title;
                    var newDueDate = request.DueDate ?? reminder.DueDate;
                    var newType = request.TypeId ?? reminder.TypeId;
                    var container = request.ContainerId ?? reminder.ContainerId;

                    if (!string.IsNullOrWhiteSpace(reminder.HangfireJobId))
                    {
                        reminderService.DeleteHangfireJob(reminder.HangfireJobId);
                    }

                    var userId = reminder.CreatedBy;
                    var email = await userQueries.GetEmailByUserId(userId!.Value, cancellationToken);
                    var newReminderTime = newDueDate.AddSeconds(-30);

                    var newJobId = reminderService.ScheduleReminder(email, newTitle, newReminderTime);

                    var updateModel = new UpdateReminderModel
                    {
                        Id = reminderId,
                        Title = newTitle,
                        DueDate = newDueDate,
                        TypeId = newType,
                        ContainerId = container,
                        HangfireJobId = newJobId,
                        ModifiedBy = userProvider.GetUserId()
                    };

                    var updatedReminder = await reminderRepository.Update(updateModel, cancellationToken);

                    return ServiceResponse.OkResponse("Reminder updated", updatedReminder);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult(
                ServiceResponse.NotFoundResponse("Reminder not found"))
        );
    }
}