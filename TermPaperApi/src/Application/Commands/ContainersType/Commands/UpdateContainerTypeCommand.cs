using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ContainerTypes;
using Domain.ContainerTypes.Models;
using MediatR;

namespace Application.Commands.ContainersType.Commands;

public record UpdateContainerTypeCommand : IRequest<ServiceResponse>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
}

public class UpdateContainerTypeCommandHandler(
    IContainerTypeRepository containerTypeRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateContainerTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var containerTypeId = request.Id;

        var existingContainerType = await containerTypeRepository.GetById(containerTypeId, cancellationToken);

        return await existingContainerType.Match(
            async container =>
            {
                try
                {
                    var updateContainerModel = new UpdateContainerTypeModel
                    {
                        Id = containerTypeId,
                        Name = request.Name,
                        ModifiedBy = userId,
                    };
                    var updatedContainer = await containerTypeRepository.Update(updateContainerModel, cancellationToken);
                    return ServiceResponse.OkResponse("Container type updated", updatedContainer);
                }
                catch (ContainerTypeException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Container type not found"))
        );
    }
}