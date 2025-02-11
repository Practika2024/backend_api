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

public record DeleteContainerCommand : IRequest<Result<ContainerDto, ContainerException>>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<DeleteContainerCommand, Result<ContainerDto, ContainerException>>
{
    public async Task<Result<ContainerDto, ContainerException>> Handle(
        DeleteContainerCommand request,
        CancellationToken cancellationToken)
    {
        var containerIdObj = new ContainerId(request.Id);
        var existingContainer = await containerRepository.GetById(containerIdObj, cancellationToken);
        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var model = new DeleteContainerModel
                    {
                        Id = container.Id
                    };
                    var deletedContainer = await containerRepository.Delete(model, cancellationToken);
                    return ContainerDto.FromDomainModel(deletedContainer);
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerIdObj, exception);
                }
            },
            () => Task.FromResult<Result<ContainerDto, ContainerException>>(
                new ContainerNotFoundException(containerIdObj))
        );
    }
}