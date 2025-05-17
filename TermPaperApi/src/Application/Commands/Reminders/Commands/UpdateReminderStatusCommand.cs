using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Reminders;
using Domain.Reminders.Models;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public class UpdateReminderStatusCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
    public required int Status { get; init; }
}

public class UpdateReminderStatusCommandHandler(
    IReminderRepository reminderRepository,
    IUserProvider userProvider,
    IMapper mapper) : IRequestHandler<UpdateReminderStatusCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateReminderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var reminderId = request.Id;
        var status = request.Status;

        if (!Enum.IsDefined(typeof(ReminderStatus), status))
        {
            return ServiceResponse.BadRequestResponse("Invalid status");
        }
        
        var existingReminder = await reminderRepository.GetById(reminderId, cancellationToken);

        return await existingReminder.Match(
            async reminder =>
            {
                try
                {
                    if (userProvider.GetUserId() != reminder.CreatedBy)
                    {
                        return ServiceResponse.ForbiddenResponse("You are not allowed to update this reminder");
                    }

                    reminder.Status = (ReminderStatus)status;
                    
                    var updatedReminder = await reminderRepository.Update(mapper.Map<UpdateReminderModel>(reminder), cancellationToken);
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