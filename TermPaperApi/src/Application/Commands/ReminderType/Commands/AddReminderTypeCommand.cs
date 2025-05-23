using System.Net;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ReminderTypes.Models;
using MediatR;

namespace Application.Commands.ReminderType.Commands;

public record AddReminderTypeCommand : IRequest<ServiceResponse>
{
    public required string Name { get; init; }
}

public class AddReminderTypeCommandHandler(IReminderTypeRepository reminderTypeRepository, IUserProvider userProvider)
    : IRequestHandler<AddReminderTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        AddReminderTypeCommand request,
        CancellationToken cancellationToken)
    {
        var existingReminderType = await reminderTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingReminderType.Match<Task<ServiceResponse>>(
            c => Task.FromResult<ServiceResponse>(
                ServiceResponse.GetResponse("Reminder type with this name already exists", false, null, HttpStatusCode.Conflict)),
            async () => { return await CreateEntity(request.Name, userProvider.GetUserId(), cancellationToken); });
    }

    private async Task<ServiceResponse> CreateEntity(
        string name,
        Guid createdBy,
        CancellationToken cancellationToken)
    {
        try
        {
            var createProductModel = new CreateReminderTypeModel
            {
                Name = name,
                CreatedBy = createdBy
            };

            var createdProduct = await reminderTypeRepository.Create(createProductModel, cancellationToken);
            return ServiceResponse.OkResponse("Reminder type", createdProduct);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
        }
    }
}