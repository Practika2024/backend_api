using Application.Commands.Containers.Exceptions;
using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerTypes;
using Domain.ContainerTypes.Models;
using MediatR;
using ContainerUnknownException = Application.Commands.ContainersType.Exceptions.ContainerUnknownException;

namespace Application.Commands.ContainersType.Commands;

public record AddContainerTypeCommand : IRequest<Result<ContainerType, ContainerTypeException>>
{
    public required string Name { get; init; }
    public Guid CreatedBy { get; set; }
}

public class AddContainerTypeCommandHandler(IContainerTypeRepository containerTypeRepository, IUserProvider userProvider)
    : IRequestHandler<AddContainerTypeCommand, Result<ContainerType, ContainerTypeException>>
{
    public async Task<Result<ContainerType, ContainerTypeException>> Handle(
        AddContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        request.CreatedBy = userProvider.GetUserId();
        
        var existingContainerType = await containerTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainerType.Match<Task<Result<ContainerType, ContainerTypeException>>>(
            c => Task.FromResult<Result<ContainerType, ContainerTypeException>>(new ContainerTypeAlreadyExists(c.Id)),
            async () =>
            {
                return await CreateEntity(request.Name, request.CreatedBy, cancellationToken);
            });
    }

    private async Task<Result<ContainerType, ContainerTypeException>> CreateEntity(
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
            return createdContainer;
        }
        catch (ContainerTypeException exception)
        {
            return new ContainerUnknownException(Guid.Empty, exception);
        }
    }
}