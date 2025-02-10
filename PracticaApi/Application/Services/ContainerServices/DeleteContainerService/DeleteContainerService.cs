using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Models;
using Application.Models.ContainerModels;
using Application.ViewModels;
using Domain.Containers;
using Optional;

namespace Application.Services.ContainerServices.DeleteContainerService;

public class DeleteContainerService : IDeleteContainerService
{
    private readonly IContainerRepository _containerRepository;

    public DeleteContainerService(IContainerRepository containerRepository)
    {
        _containerRepository = containerRepository;
    }

    public async Task<Result<ContainerVM, ContainerException>> DeleteContainerAsync(
        Guid containerId,
        CancellationToken cancellationToken)
    {
        var containerIdObj = new ContainerId(containerId);
        var existingContainer = await _containerRepository.GetById(containerIdObj, cancellationToken);

        return await existingContainer.Match(
            async container =>
            {
                try
                {
                    var deletedContainer = await _containerRepository.Delete(new DeleteContainerModel
                    {
                        Id = container.Id
                    }, cancellationToken);
                    return new ContainerVM(deletedContainer);
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerIdObj, exception);
                }
            },
            () => Task.FromResult<Result<ContainerVM, ContainerException>>(
                new ContainerNotFoundException(containerIdObj))
        );
    }
}