using System.Security.Cryptography;
using Application.Commands.Containers.Exceptions;
using Application.Common;
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

    public class AddContainerCommandHandler(
        IContainerRepository containerRepository) : IRequestHandler<AddContainerCommand, Result<ContainerEntity, ContainerException>>
    {
        public async Task<Result<ContainerEntity, ContainerException>> Handle(
            AddContainerCommand request,
            CancellationToken cancellationToken)
        {
            var userId = request.UserId;
            var typeId = request.TypeId;
            var existingContainer = await containerRepository.SearchByName(request.Name, cancellationToken);

            return await existingContainer.Match<Task<Result<ContainerEntity, ContainerException>>>(
                c => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                    new ContainerAlreadyExistsException(c.Id)),
                async () => await CreateEntity(
                    request.Name,
                    request.Volume,
                    userId,
                    request.Notes,
                    typeId,
                    cancellationToken));
        }

        private async Task<Result<ContainerEntity, ContainerException>> CreateEntity(
            string name,
            decimal volume,
            Guid userId,
            string? notes,
            Guid typeId,
            CancellationToken cancellationToken)
        {
            try
            {
                //var uniqueCode = await GenerateUniqueCodeAsync(name, typeId, containerRepository, cancellationToken);
                var uniqueCode = await GenerateUniqueCodeAsync(containerRepository, cancellationToken);
                var containerId = Guid.NewGuid();
                var createContainerModel = new CreateContainerModel
                {
                    Id = containerId,
                    Name = name,
                    Volume = volume,
                    Notes = notes,
                    CreatedBy = userId,
                    TypeId = typeId,
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

        // private async Task<string> GenerateUniqueCodeAsync(
        //     string containerName,
        //     Guid typeId,
        //     IContainerRepository containerRepository,
        //     CancellationToken cancellationToken)
        // {
        //     var containerType = await containerRepository.GetById(typeId, cancellationToken);
        //     
        //     var namePrefix = containerName.Length >= 3 
        //         ? containerName.Substring(0, 3).ToUpper() 
        //         : containerName.ToUpper();
        //     
        //     var typeLetter = containerType.Name.Substring(0, 1).ToUpper();
        //     
        //     var codePrefix = namePrefix + typeLetter;
        //     
        //     var lastSequence = await containerRepository.GetLastSequenceForPrefixAsync(codePrefix, cancellationToken);
        //     var nextSequence = lastSequence + 1;
        //     
        //     var sequenceString = nextSequence.ToString("D3");
        //     return codePrefix + sequenceString;
        // }
        
        private async Task<string> GenerateUniqueCodeAsync(
            IContainerRepository containerRepository,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var uniqueCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16))
                    .Replace("/", "_")
                    .Replace("+", "-");

                var existingContainer = await containerRepository.SearchByUniqueCode(uniqueCode, cancellationToken);
                if (!existingContainer.HasValue)
                {
                    return uniqueCode;
                }
            }
        }
    }