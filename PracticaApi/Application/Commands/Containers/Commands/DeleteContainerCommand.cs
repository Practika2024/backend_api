using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record DeleteContainerCommand : IRequest<Result<Container, ContainerException>>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<DeleteContainerCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
        DeleteContainerCommand request,
        CancellationToken cancellationToken)
    {
        var containerIdObj = request.Id;
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
                    return deletedContainer;
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerIdObj, exception);
                }
            },
            () => Task.FromResult<Result<Container, ContainerException>>(
                new ContainerNotFoundException(containerIdObj))
        );
    }
}