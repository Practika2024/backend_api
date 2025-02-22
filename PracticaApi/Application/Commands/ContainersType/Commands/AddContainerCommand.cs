using Application.Commands.Containers.Exceptions;
using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerTypeModels;
using MediatR;
using ContainerUnknownException = Application.Commands.ContainersType.Exceptions.ContainerUnknownException;

namespace Application.Commands.ContainersType.Commands;

public record AddContainerTypeCommand : IRequest<Result<ContainerType, ContainerTypeException>>
{
    public required string Name { get; init; }
    public Guid CreatedBy { get; init; }
}

public class AddContainerTypeCommandHandler(IContainerTypeRepository containerTypeRepository)
    : IRequestHandler<AddContainerTypeCommand, Result<ContainerType, ContainerTypeException>>
{
    public async Task<Result<ContainerType, ContainerTypeException>> Handle(
        AddContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        var existingContainerType = await containerTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainerType.Match<Task<Result<ContainerType, ContainerTypeException>>>(
            c => throw new Exception("Container already exists"),
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