using Application.Commands.ContainersType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ContainerTypes;
using MediatR;

namespace Application.Commands.ContainersType.Commands;

public record DeleteContainerTypeCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerTypeCommandHandler(
    IContainerTypeRepository containerTypeRepository)
    : IRequestHandler<DeleteContainerTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
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
                    return ServiceResponse.OkResponse("Container type deleted", deletedContainerType);
                }
                catch (ContainerTypeException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Container type not found"))
        );
    }
}