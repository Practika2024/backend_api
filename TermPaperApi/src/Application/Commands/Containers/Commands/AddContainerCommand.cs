using System.Net;
using Application.Commands.Containers.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Containers.Models;
using Domain.ContainerTypes;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record AddContainerCommand : IRequest<ServiceResponse>
{
    public required string Name { get; init; }
    public required decimal Volume { get; init; }
    public required string? Notes { get; init; }
    public required Guid TypeId { get; init; }
}

public class AddContainerCommandHandler(
    IContainerRepository containerRepository,
    IContainerTypeQueries containerTypeQueries,
    IUserQueries userQueries,
    IUserProvider userProvider)
    : IRequestHandler<AddContainerCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        AddContainerCommand request,
        CancellationToken cancellationToken)
    {
        var existingContainer = await containerRepository.SearchByName(request.Name, cancellationToken);

        return await existingContainer.Match<Task<ServiceResponse>>(
            c => Task.FromResult<ServiceResponse>(
                ServiceResponse.GetResponse("Container with this name already exists", false, null,
                    HttpStatusCode.Conflict)),
            async () =>
            {
                var userResult = await userQueries.GetById(userProvider.GetUserId(), cancellationToken);
                return await userResult.Match<Task<ServiceResponse>>(
                    async user =>
                    {
                        var typeResult = await containerTypeQueries.GetById(request.TypeId, cancellationToken);
                        return await typeResult.Match<Task<ServiceResponse>>(
                            async type =>
                            {
                                return await CreateEntity(
                                    request.Name,
                                    request.Volume,
                                    request.Notes,
                                    type,
                                    cancellationToken);
                            },
                            () => Task.FromResult<ServiceResponse>(
                                ServiceResponse.NotFoundResponse("Container type not found"))
                        );
                    },
                    () => Task.FromResult<ServiceResponse>(
                        ServiceResponse.NotFoundResponse("User not found"))
                );
            });
    }

    private async Task<ServiceResponse> CreateEntity(
        string name,
        decimal volume,
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
                CreatedBy = userProvider.GetUserId(),
                TypeId = type.Id,
                UniqueCode = uniqueCode
            };

            var createdContainer = await containerRepository.Create(createContainerModel, cancellationToken);
            return ServiceResponse.OkResponse("Container created", createdContainer);
        }
        catch (ContainerException exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
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