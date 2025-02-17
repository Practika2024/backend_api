using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using Domain.Containers;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record DeleteContainerCommand : IRequest<Result<ContainerEntity, ContainerException>>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<DeleteContainerCommand, Result<ContainerEntity, ContainerException>>
{
    public async Task<Result<ContainerEntity, ContainerException>> Handle(
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
            () => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                new ContainerNotFoundException(containerIdObj))
        );
    }
}