using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record ClearContainerContentCommand : IRequest<Result<Container, ContainerException>>
{
    public required Guid ContainerId { get; init; } 
    public required Guid ModifiedBy { get; init; }
}

public class ClearContainerContentCommandHandler(
    IContainerRepository containerRepository,
    IContainerQueries containerQueries,
    IContainerHistoryRepository containerHistoryRepository) : IRequestHandler<ClearContainerContentCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
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
                    if (!container.ProductId.HasValue)
                    {
                        throw new Exception("Container already is empty");
                    }
                    
                    var userId = request.ModifiedBy;

                    var clearContainerContentModel = new ClearContainerContentModel
                    {
                        ContainerId = containerId,
                        ModifiedBy = userId
                    };
                
                    await UpdateHistory(container.Id, cancellationToken);
                    
                    var updatedContainer = await containerRepository.ClearContainerContent(clearContainerContentModel, cancellationToken);
                    containerHistoryRepository.ClearContainerContent(updatedContainer.Id, userId, cancellationToken);
                    return updatedContainer;
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerId, exception);
                }
            },
            () => Task.FromResult<Result<Container, ContainerException>>(
                new ContainerNotFoundException(containerId))
        );
    }

    private async Task UpdateHistory(Guid containerId, CancellationToken cancellationToken)
    {
        await containerHistoryRepository.Update(containerId, cancellationToken);
    }
}