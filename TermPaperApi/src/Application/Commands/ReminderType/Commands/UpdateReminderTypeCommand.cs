using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ReminderTypes.Models;
using MediatR;

namespace Application.Commands.ReminderType.Commands;

public record UpdateReminderTypeCommand : IRequest<ServiceResponse>
{
    public int Id { get; set; }
    public string Name { get; init; }
}

public class UpdateReminderTypeCommandHandler(
    IReminderTypeRepository reminderTypeRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateReminderTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateReminderTypeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var reminderTypeId = request.Id;

        var existingReminderType = await reminderTypeRepository.GetById(reminderTypeId, cancellationToken);

        return await existingReminderType.Match(
            async reminder =>
            {
                try
                {
                    var updateReminderModel = new UpdateReminderTypeModel
                    {
                        Id = reminderTypeId,
                        Name = request.Name,
                        ModifiedBy = userId,
                    };
                    var updatedReminder = await reminderTypeRepository.Update(updateReminderModel, cancellationToken);
                    return ServiceResponse.OkResponse("Reminder type updated", updatedReminder);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Reminder type not found", null))
        );
    }
}