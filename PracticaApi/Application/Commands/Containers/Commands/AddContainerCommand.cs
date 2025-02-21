using System.Security.Cryptography;
using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using Domain.Containers;
using MediatR;
using Microsoft.AspNetCore.StaticFiles;

namespace Application.Commands.Containers.Commands;

public record AddContainerCommand : IRequest<Result<ContainerEntity, ContainerException>>
{
    public required string Name { get; init; }
    public required decimal Volume { get; init; }
    public required string? Notes { get; init; }
    public required Guid UserId { get; init; }
    public required Guid TypeId { get; init; }
}

public class AddContainerCommandHandler : IRequestHandler<AddContainerCommand, Result<ContainerEntity, ContainerException>>
{
    private readonly IContainerRepository containerRepository;
    private readonly IContainerTypeQueries containerTypeQueries;
    private readonly IUserQueries userQueries;

    public AddContainerCommandHandler(
        IContainerRepository containerRepository,
        IContainerTypeQueries containerTypeQueries,
        IUserQueries userQueries)
    {
        this.containerRepository = containerRepository;
        this.containerTypeQueries = containerTypeQueries;
        this.userQueries = userQueries;
    }

    public async Task<Result<ContainerEntity, ContainerException>> Handle(
        AddContainerCommand request,
        CancellationToken cancellationToken)
    {
        var existingContainer = await containerRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainer.Match<Task<Result<ContainerEntity, ContainerException>>>(
            c => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                new ContainerAlreadyExistsException(c.Id)),
            async () =>
            {
                var userResult = await userQueries.GetById(request.UserId, cancellationToken);
                return await userResult.Match<Task<Result<ContainerEntity, ContainerException>>>(
                    async user =>
                    {
                        var typeResult = await containerTypeQueries.GetById(request.TypeId, cancellationToken);
                        return await typeResult.Match<Task<Result<ContainerEntity, ContainerException>>>(
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
                            () => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                                new ContainerTypeNotFoundException(request.TypeId))
                        );
                    },
                    () => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                        new UserNotFoundException(request.UserId))
                );
            });
    }

    private async Task<Result<ContainerEntity, ContainerException>> CreateEntity(
        string name,
        decimal volume,
        Guid userId,
        string? notes,
        ContainerTypeEntity type,
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
        ContainerTypeEntity type,
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