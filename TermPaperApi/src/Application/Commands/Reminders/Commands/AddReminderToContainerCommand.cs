using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ReminderService;
using Domain.Reminders.Models;
using MediatR;
using Optional.Unsafe;

namespace Application.Commands.Reminders.Commands;

public record AddReminderToContainerCommand : IRequest<ServiceResponse>
{
    public required Guid ContainerId { get; init; }
    public required string Title { get; init; } = null!;
    public required DateTime DueDate { get; init; }
    public required int TypeId { get; init; }
}

public class AddReminderToContainerCommandHandler(
    IContainerRepository containerRepository,
    IReminderRepository reminderRepository,
    IUserProvider userProvider,
    IReminderService reminderService,
    IUserQueries userQueries)
    : IRequestHandler<AddReminderToContainerCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
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
                        TypeId = request.TypeId,
                        CreatedBy = userId,
                        HangfireJobId = null
                    };
                    
                    string jobId = string.Empty;

                    if ((await userQueries.GetById(userId, cancellationToken)).ValueOrDefault().EmailConfirmed)
                    {
                        // Плануємо нагадування через Hangfire
                        // var reminderTime = createdReminder.DueDate.AddMinutes(-30);
                        var reminderTime = createReminderModel.DueDate.AddSeconds(-30);
                        jobId = reminderService.ScheduleReminder(await userQueries.GetEmailByUserId(userId, cancellationToken),
                            createReminderModel.Title, reminderTime);
                        
                        createReminderModel.HangfireJobId = jobId;
                    }
                    
                    var createdReminder = await reminderRepository.Create(createReminderModel, cancellationToken);

                    return ServiceResponse.OkResponse("Reminder", createdReminder);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult(
                ServiceResponse.NotFoundResponse("Container not found"))
        );
    }
}