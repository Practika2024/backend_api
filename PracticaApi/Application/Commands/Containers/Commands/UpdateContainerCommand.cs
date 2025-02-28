﻿using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;

namespace Application.Commands.Containers.Commands;

public record UpdateContainerCommand : IRequest<Result<Container, ContainerException>>
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public decimal Volume { get; init; }
    public string? Notes { get; init; }
    public Guid ModifiedBy { get; init; }
}

public class UpdateContainerCommandHandler(
    IContainerRepository containerRepository,
    IMapper mapper)
    : IRequestHandler<UpdateContainerCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
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
                    var updatedContainerModel = new UpdateContainerModel
                    {
                        Id = request.Id,
                        Name = container.Name,
                        Notes = request.Notes,
                        Volume = request.Volume,
                        ModifiedBy = userId,
                    };

                    updatedContainerModel.ModifiedBy = request.ModifiedBy;

                    var updatedContainer = await containerRepository.Update(updatedContainerModel, cancellationToken);
                    return updatedContainer;
                }
                catch (ContainerException exception)
                {
                    return new ContainerUnknownException(containerId, exception);
                }
            },
            () => Task.FromResult<Result<Container, ContainerException>>(
                new ContainerNotFoundException(containerId))
        );
    }
}