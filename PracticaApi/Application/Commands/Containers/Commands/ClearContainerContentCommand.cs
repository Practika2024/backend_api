using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using Domain.Containers;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record ClearContainerContentCommand : IRequest<Result<ContainerEntity, ContainerException>>
{
    public required Guid ContainerId { get; init; } 
    public required Guid ModifiedBy { get; init; }
}

public class ClearContainerContentCommandHandler(
    IContainerRepository containerRepository) : IRequestHandler<ClearContainerContentCommand, Result<ContainerEntity, ContainerException>>
{
    public async Task<Result<ContainerEntity, ContainerException>> Handle(
        ClearContainerContentCommand request,
        CancellationToken cancellationToken)
    {
        var containerId = request.ContainerId;
        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var userId = request.ModifiedBy;

                    var clearContainerContentModel = new ClearContainerContentModel
                    {
                        ContainerId = containerId,
                        ModifiedBy = userId
                    };

                    var updatedContainer = await containerRepository.ClearContainerContent(clearContainerContentModel, cancellationToken);
                    return updatedContainer;
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerId, exception);
                }
            },
            () => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                new ContainerNotFoundException(containerId))
        );
    }
}