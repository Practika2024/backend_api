using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerModels;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record SetContainerContentCommand : IRequest<Result<Container, ContainerException>>
{
    public required Guid ContainerId { get;  init; } 
    public required Guid? ProductId { get; init; }
    public required Guid ModifiedBy { get;  init; }
}

public class SetContainerContentCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<SetContainerContentCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
        SetContainerContentCommand request,
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
                    var productId = request.ProductId != null
                        ? request.ProductId 
                        : null;

                    var setContainerContentModel = new SetContainerContentModel
                    {
                        ContainerId = containerId,
                        ProductId = productId,
                        ModifiedBy = userId
                    };

                    var updatedContainer =
                        await containerRepository.SetContainerContent(setContainerContentModel, cancellationToken);
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
}