using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.ContainerModels;
using Domain.Containers;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record UpdateContainerCommand : IRequest<Result<ContainerEntity, ContainerException>>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public decimal Volume { get; init; } 
    public string? Notes { get; init; } 
    public Guid ModifiedBy { get; init; }
    public Guid TypeId { get; init; }
}
public class UpdateContainerCommandHandler(
        IContainerRepository containerRepository) : IRequestHandler<UpdateContainerCommand, Result<ContainerEntity, ContainerException>>
    {
        public async Task<Result<ContainerEntity, ContainerException>> Handle(
            UpdateContainerCommand request,
            CancellationToken cancellationToken)
        {
            var userId = request.ModifiedBy;
            var containerId = request.Id;

            var existingContainer = await containerRepository.GetById(containerId, cancellationToken);

            return await existingContainer.Match(
                async container =>
                {
                    try
                    {
                        var typeId = request.TypeId;
                        var updateContainerModel = new UpdateContainerModel
                        {
                            Id = containerId,
                            Name = request.Name,
                            Volume = request.Volume,
                            Notes = request.Notes,
                            ModifiedBy = userId,
                            TypeId = typeId,
                        };
                        var updatedContainer = await containerRepository.Update(updateContainerModel, cancellationToken);
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