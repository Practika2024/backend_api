using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record DeleteContainerCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteContainerCommandHandler(
    IContainerRepository containerRepository)
    : IRequestHandler<DeleteContainerCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        DeleteContainerCommand request,
        CancellationToken cancellationToken)
    {
        var containerIdObj = request.Id;
        var existingContainer = await containerRepository.GetById(containerIdObj, cancellationToken);
        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var model = new DeleteContainerModel
                    {
                        Id = container.Id
                    };
                    var deletedContainer = await containerRepository.Delete(model, cancellationToken);
                    return ServiceResponse.OkResponse("Container deleted", deletedContainer);
                }
                catch (ContainerException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Container not found"))
        );
    }
}