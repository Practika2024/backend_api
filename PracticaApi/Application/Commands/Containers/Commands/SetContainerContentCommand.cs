using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using Domain.Containers;
using Domain.Products;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record SetContainerContentCommand : IRequest<Result<ContainerEntity, ContainerException>>
{
    public required Guid ContainerId { get;  init; } 
    public required Guid? ProductId { get; init; }
    public required bool IsEmpty { get;  init; } 
    public required Guid ModifiedBy { get;  init; }
}

public class SetContainerContentCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<SetContainerContentCommand, Result<ContainerEntity, ContainerException>>
{
    public async Task<Result<ContainerEntity, ContainerException>> Handle(
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
                        IsEmpty = request.IsEmpty,
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
            () => Task.FromResult<Result<ContainerEntity, ContainerException>>(
                new ContainerNotFoundException(containerId))
        );
    }
}