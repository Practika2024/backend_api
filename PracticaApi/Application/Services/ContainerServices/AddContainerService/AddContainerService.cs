using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Containers;
using Optional;
using System.Security.Cryptography;
using Application.Common;
using Domain.Authentications.Users;

namespace Application.Services.ContainerServices.AddContainerService;
public class AddContainerService : IAddContainerService
{
    private readonly IContainerRepository _containerRepository;

    public AddContainerService(IContainerRepository containerRepository)
    {
        _containerRepository = containerRepository;
    }

    public async Task<Result<ContainerVM, ContainerException>> AddContainerAsync(
        string name,
        decimal volume,
        UserId userId,
        string? notes,
        ContainerType type,
        CancellationToken cancellationToken)
    {
        var existingContainer = await _containerRepository.SearchByName(name, cancellationToken);

        return await existingContainer.Match<Task<Result<ContainerVM, ContainerException>>>(
            c => Task.FromResult<Result<ContainerVM, ContainerException>>(new ContainerAlreadyExistsException(c.Id)), 
            async () => await CreateEntity(name, volume,userId, notes, type, cancellationToken)
        );
    }

    private async Task<Result<ContainerVM, ContainerException>> CreateEntity(
        string name,
        decimal volume,
        UserId userId,
        string? notes,
        ContainerType type,
        CancellationToken cancellationToken)
    {
        try
        {
            var uniqueCode = await GenerateUniqueCodeAsync(cancellationToken);

            var containerId = ContainerId.New();
            var newContainer = ContainerEntity.New(
                containerId,
                name,
                volume,
                notes,
                true,
                userId,
                type,
                uniqueCode);

            var createdContainer = await _containerRepository.Create(newContainer, cancellationToken);
            return new ContainerVM(createdContainer);
        }
        catch (ContainerException exception)
        {
            return new ContainerUnknownException(ContainerId.Empty, exception);
        }
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var uniqueCode = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16))
                .Replace("/", "_")
                .Replace("+", "-");
            var existingContainer = await _containerRepository.SearchByUniqueCode(uniqueCode, cancellationToken);
            if (!existingContainer.HasValue)
            {
                return uniqueCode;
            }
        }
    }
}