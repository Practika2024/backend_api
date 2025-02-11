using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Dtos.Containers;
using Application.Models.ContainerModels;
using Domain.Authentications.Users;
using Domain.Containers;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record ClearContainerContentCommand : IRequest<Result<ContainerDto, ContainerException>>
{
    public required Guid ContainerId { get; init; } 
    public required Guid ModifiedBy { get; init; }
}

public class ClearContainerContentCommandHandler(
    IContainerRepository containerRepository) : IRequestHandler<ClearContainerContentCommand, Result<ContainerDto, ContainerException>>
{
    public async Task<Result<ContainerDto, ContainerException>> Handle(
        ClearContainerContentCommand request,
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

                    var clearContainerContentModel = new ClearContainerContentModel
                    {
                        ContainerId = containerId,
                        ModifiedBy = userId
                    };

                    var updatedContainer = await containerRepository.ClearContainerContent(clearContainerContentModel, cancellationToken);
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