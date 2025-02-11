using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Dtos.Containers;
using Application.Models.ContainerModels;
using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Products;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record SetContainerContentCommand : IRequest<Result<ContainerDto, ContainerException>>
{
    public required Guid ContainerId { get;  init; } 
    public required Guid? ProductId { get; init; }
    public required bool IsEmpty { get;  init; } 
    public required Guid ModifiedBy { get;  init; }
}

public class SetContainerContentCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<SetContainerContentCommand, Result<ContainerDto, ContainerException>>
{
    public async Task<Result<ContainerDto, ContainerException>> Handle(
        SetContainerContentCommand request,
        CancellationToken cancellationToken)
    {
        var containerId = new ContainerId(request.ContainerId);
        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var userId = new UserId(request.ModifiedBy);
                    var productId = request.ProductId != null
                        ? new ProductId(request.ProductId.Value)
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
                    return ContainerDto.FromDomainModel(updatedContainer);
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerId, exception);
                }
            },
            () => Task.FromResult<Result<ContainerDto, ContainerException>>(
                new ContainerNotFoundException(containerId))
        );
    }
}