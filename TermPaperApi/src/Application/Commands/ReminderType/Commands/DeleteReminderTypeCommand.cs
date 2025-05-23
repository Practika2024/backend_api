using Application.Common.Interfaces.Repositories;
using Application.Services;
using MediatR;

namespace Application.Commands.ReminderType.Commands;

public record DeleteReminderTypeCommand : IRequest<ServiceResponse>
{
    public required int Id { get; init; }
}

public class DeleteReminderTypeCommandHandler(
    IReminderTypeRepository reminderTypeRepository)
    : IRequestHandler<DeleteReminderTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        DeleteReminderTypeCommand request,
        CancellationToken cancellationToken)
    {
        var reminderTypeIdObj = request.Id;
        var existingReminderType = await reminderTypeRepository.GetById(reminderTypeIdObj, cancellationToken);
        return await existingReminderType.Match(
            async reminderType =>
            {
                try
                {
                    var deletedReminderType = await reminderTypeRepository.Delete(reminderType.Id, cancellationToken);
                    return ServiceResponse.OkResponse("Product type deleted", deletedReminderType);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Product type not found"))
        );
    }
}