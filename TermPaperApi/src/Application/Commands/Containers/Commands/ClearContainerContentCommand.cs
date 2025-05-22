using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record ClearContainerContentCommand : IRequest<ServiceResponse>
{
    public required Guid ContainerId { get; init; }
}

public class ClearContainerContentCommandHandler(
    IContainerRepository containerRepository,
    IContainerQueries containerQueries,
    IContainerHistoryRepository containerHistoryRepository,
    IUserProvider userProvider) : IRequestHandler<ClearContainerContentCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        ClearContainerContentCommand request,
        CancellationToken cancellationToken)
    {
        var containerId = request.ContainerId;
        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    if (!container.ProductId.HasValue)
                    {
                        throw new Exception("Container already is empty");
                    }
                    
                    var userId = userProvider.GetUserId();

                    var clearContainerContentModel = new ClearContainerContentModel
                    {
                        ContainerId = containerId,
                        ModifiedBy = userId
                    };
                
                    await UpdateHistory(container.Id, cancellationToken);
                    
                    var updatedContainer = await containerRepository.ClearContainerContent(clearContainerContentModel, cancellationToken);
                    return ServiceResponse.OkResponse("Container content cleared", updatedContainer);
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

    private async Task UpdateHistory(Guid containerId, CancellationToken cancellationToken)
    {
        await containerHistoryRepository.Update(containerId, cancellationToken);
    }
}