using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerTypes;
using MediatR;

namespace Application.Commands.ContainersType.Commands;

public record DeleteContainerTypeCommand : IRequest<Result<ContainerType, ContainerTypeException>>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerTypeCommandHandler(
    IContainerTypeRepository containerTypeRepository)
    : IRequestHandler<DeleteContainerTypeCommand, Result<ContainerType, ContainerTypeException>>
{
    public async Task<Result<ContainerType, ContainerTypeException>> Handle(
        DeleteContainerTypeCommand request,
        CancellationToken cancellationToken)
    {
        var containerTypeIdObj = request.Id;
        var existingContainerType = await containerTypeRepository.GetById(containerTypeIdObj, cancellationToken);
        return await existingContainerType.Match(
            async containerType =>
            {
                try
                {
                    var deletedContainerType = await containerTypeRepository.Delete(containerType.Id, cancellationToken);
                    return deletedContainerType;
                }
                catch (ContainerTypeException exception)
                {
                    return new ContainerUnknownException(containerTypeIdObj, exception);
                }
            },
            () => Task.FromResult<Result<ContainerType, ContainerTypeException>>(
                new ContainerTypeNotFoundException(containerTypeIdObj))
        );
    }
}