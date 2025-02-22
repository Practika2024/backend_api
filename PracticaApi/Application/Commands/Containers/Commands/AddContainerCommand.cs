using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerModels;
using Domain.ContainerTypeModels;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record AddContainerCommand : IRequest<Result<Container, ContainerException>>
{
    public required string Name { get; init; }
    public required decimal Volume { get; init; }
    public required string? Notes { get; init; }
    public required Guid UserId { get; init; }
    public required Guid TypeId { get; init; }
}

public class AddContainerCommandHandler(
    IContainerRepository containerRepository,
    IContainerTypeQueries containerTypeQueries,
    IUserQueries userQueries)
    : IRequestHandler<AddContainerCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
        AddContainerCommand request,
        CancellationToken cancellationToken)
    {
        var existingContainer = await containerRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainer.Match<Task<Result<Container, ContainerException>>>(
            c => Task.FromResult<Result<Container, ContainerException>>(
                new ContainerAlreadyExistsException(c.Id)),
            async () =>
            {
                var userResult = await userQueries.GetById(request.UserId, cancellationToken);
                return await userResult.Match<Task<Result<Container, ContainerException>>>(
                    async user =>
                    {
                        var typeResult = await containerTypeQueries.GetById(request.TypeId, cancellationToken);
                        return await typeResult.Match<Task<Result<Container, ContainerException>>>(
                            async type =>
                            {
                                return await CreateEntity(
                                    request.Name,
                                    request.Volume,
                                    request.UserId,
                                    request.Notes,
                                    type,
                                    cancellationToken);
                            },
                            () => Task.FromResult<Result<Container, ContainerException>>(
                                new ContainerTypeNotFoundException(request.TypeId))
                        );
                    },
                    () => Task.FromResult<Result<Container, ContainerException>>(
                        new UserNotFoundException(request.UserId))
                );
            });
    }

    private async Task<Result<Container, ContainerException>> CreateEntity(
        string name,
        decimal volume,
        Guid userId,
        string? notes,
        ContainerType type,
        CancellationToken cancellationToken)
    {
        try
        {
            var uniqueCode = await GenerateUniqueCodeAsync(name, type, cancellationToken);
            var containerId = Guid.NewGuid();
            var createContainerModel = new CreateContainerModel
            {
                Id = containerId,
                Name = name,
                Volume = volume,
                Notes = notes,
                CreatedBy = userId,
                TypeId = type.Id,
                UniqueCode = uniqueCode
            };

            var createdContainer = await containerRepository.Create(createContainerModel, cancellationToken);
            return createdContainer;
        }
        catch (ContainerException exception)
        {
            return new ContainerUnknownException(Guid.Empty, exception);
        }
    }

    private async Task<string> GenerateUniqueCodeAsync(
        string containerName,
        ContainerType type,
        CancellationToken cancellationToken)
    {
        var namePrefix = containerName.Length >= 3
            ? containerName.Substring(0, 3).ToUpper()
            : containerName.ToUpper();

        var typeLetter = type.Name.Substring(0, 1).ToUpper();

        var codePrefix = namePrefix + typeLetter;

        var lastSequence = await containerRepository.GetLastSequenceForPrefixAsync(codePrefix, cancellationToken);
        var nextSequence = lastSequence + 1;

        var sequenceString = nextSequence.ToString("D3");
        return codePrefix + sequenceString;
    }
}