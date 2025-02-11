using System.Security.Cryptography;
using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Dtos.Containers;
using Application.Models.ContainerModels;
using Domain.Authentications.Users;
using Domain.Containers;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record AddContainerCommand : IRequest<Result<ContainerDto, ContainerException>>
    {
        public required string Name { get; init; }
        public required decimal Volume { get; init; }
        public required string? Notes { get; init; }
        public required Guid UserId { get; init; }
        public required Guid TypeId { get; init; }
    }

    public class AddContainerCommandHandler(
        IContainerRepository containerRepository) : IRequestHandler<AddContainerCommand, Result<ContainerDto, ContainerException>>
    {
        public async Task<Result<ContainerDto, ContainerException>> Handle(
            AddContainerCommand request,
            CancellationToken cancellationToken)
        {
            var userId = new UserId(request.UserId);
            var typeId = new ContainerTypeId(request.TypeId);
            var existingContainer = await containerRepository.SearchByName(request.Name, cancellationToken);

            return await existingContainer.Match<Task<Result<ContainerDto, ContainerException>>>(
                c => Task.FromResult<Result<ContainerDto, ContainerException>>(
                    new ContainerAlreadyExistsException(c.Id)),
                async () => await CreateEntity(
                    request.Name,
                    request.Volume,
                    userId,
                    request.Notes,
                    typeId,
                    cancellationToken));
        }

        private async Task<Result<ContainerDto, ContainerException>> CreateEntity(
            string name,
            decimal volume,
            UserId userId,
            string? notes,
            ContainerTypeId typeId,
            CancellationToken cancellationToken)
        {
            try
            {
                var uniqueCode = await GenerateUniqueCodeAsync(containerRepository, cancellationToken);
                var containerId = ContainerId.New();
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
                return ContainerDto.FromDomainModel(createdContainer);
            }
            catch (ContainerException exception)
            {
                return new ContainerUnknownException(ContainerId.Empty, exception);
            }
        }

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