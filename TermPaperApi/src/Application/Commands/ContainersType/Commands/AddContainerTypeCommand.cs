using System.Net;
using Application.Commands.Containers.Exceptions;
using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ContainerTypes;
using Domain.ContainerTypes.Models;
using MediatR;
using ContainerUnknownException = Application.Commands.ContainersType.Exceptions.ContainerUnknownException;

namespace Application.Commands.ContainersType.Commands;

public record AddContainerTypeCommand : IRequest<ServiceResponse>
{
    public required string Name { get; init; }
    public Guid CreatedBy { get; set; }
}

public class AddContainerTypeCommandHandler(IContainerTypeRepository containerTypeRepository, IUserProvider userProvider)
    : IRequestHandler<AddContainerTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        AddContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        request.CreatedBy = userProvider.GetUserId();
        
        var existingContainerType = await containerTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainerType.Match<Task<ServiceResponse>>(
            c => Task.FromResult<ServiceResponse>(ServiceResponse.GetResponse("Container type with this name already exists", false, null, HttpStatusCode.Conflict)),
            async () =>
            {
                return await CreateEntity(request.Name, request.CreatedBy, cancellationToken);
            });
    }

    private async Task<ServiceResponse> CreateEntity(
        string name,
        Guid createdBy,
        CancellationToken cancellationToken)
    {
        try
        {
            var containerId = Guid.NewGuid();
            var createContainerModel = new CreateContainerTypeModel
            {
                Id = containerId,
                Name = name,
                CreatedBy = createdBy
            };

            var createdContainer = await containerTypeRepository.Create(createContainerModel, cancellationToken);
            return ServiceResponse.OkResponse("Container type", createdContainer);
        }
        catch (ContainerTypeException exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
        }
    }
}