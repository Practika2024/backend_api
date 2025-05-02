using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record UpdateContainerCommand : IRequest<ServiceResponse>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Volume { get; init; }
    public string? Notes { get; init; }
}

public class UpdateContainerCommandHandler(
    IContainerRepository containerRepository,
    IMapper mapper, IUserProvider userProvider)
    : IRequestHandler<UpdateContainerCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateContainerCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var containerId = request.Id;

        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var updatedContainerModel = new UpdateContainerModel
                    {
                        Id = request.Id,
                        Name = request.Name,
                        Notes = request.Notes,
                        Volume = request.Volume,
                        ModifiedBy = userId,
                    };

                    updatedContainerModel.ModifiedBy = userProvider.GetUserId();

                    var updatedContainer = await containerRepository.Update(updatedContainerModel, cancellationToken);
                    return ServiceResponse.OkResponse("Container updated", updatedContainer);
                }
                catch (ContainerException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Container not found"))
        );
    }
}