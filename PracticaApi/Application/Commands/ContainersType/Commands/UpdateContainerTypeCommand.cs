using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerModels;
using Domain.ContainerTypeModels;
using MediatR;

namespace Application.Commands.ContainersType.Commands;

public record UpdateContainerTypeCommand : IRequest<Result<ContainerType, ContainerTypeException>>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public Guid ModifiedBy { get; init; }
}

public class UpdateContainerTypeCommandHandler(
    IContainerTypeRepository containerTypeRepository)
    : IRequestHandler<UpdateContainerTypeCommand, Result<ContainerType, ContainerTypeException>>
{
    public async Task<Result<ContainerType, ContainerTypeException>> Handle(
        UpdateContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.ModifiedBy;
        var containerTypeId = request.Id;

        var existingContainerType = await containerTypeRepository.GetById(containerTypeId, cancellationToken);

        return await existingContainerType.Match(
            async container =>
            {
                try
                {
                    var updateContainerModel = new UpdateContainerTypeModel
                    {
                        Id = containerTypeId,
                        Name = request.Name,
                        ModifiedBy = userId,
                    };
                    var updatedContainer = await containerTypeRepository.Update(updateContainerModel, cancellationToken);
                    return updatedContainer;
                }
                catch (ContainerTypeException exception)
                {
                    return new ContainerUnknownException(containerTypeId, exception);
                }
            },
            () => Task.FromResult<Result<ContainerType, ContainerTypeException>>(
                new ContainerTypeNotFoundException(containerTypeId))
        );
    }
}