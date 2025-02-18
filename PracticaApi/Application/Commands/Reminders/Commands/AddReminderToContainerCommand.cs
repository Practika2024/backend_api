﻿using Application.Commands.Reminders.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.ReminderModels;
using Domain.Containers;
using Domain.Reminders;
using MediatR;

namespace Application.Commands.Reminders.Commands;

public record AddReminderToContainerCommand : IRequest<Result<ReminderEntity, ReminderException>>
{
    public required Guid ContainerId { get; init; }
    public required string Title { get; init; } = null!; 
    public required DateTime DueDate { get; init; } 
    public required ReminderType Type { get; init; } 
    public required Guid CreatedBy { get; init; } 
}
 public class AddReminderToContainerCommandHandler(
        IContainerRepository containerRepository,
        IReminderRepository reminderRepository) : IRequestHandler<AddReminderToContainerCommand, Result<ReminderEntity, ReminderException>>
    {
        public async Task<Result<ReminderEntity, ReminderException>> Handle(
            AddReminderToContainerCommand request,
            CancellationToken cancellationToken)
        {
            var containerId = request.ContainerId;
            var existingContainer = await containerRepository.GetById(containerId, cancellationToken);
            var reminderId = Guid.NewGuid();

            return await existingContainer.Match(
                async container =>
                {
                    try
                    {
                        var userId = request.CreatedBy;
                        var createReminderModel = new CreateReminderModel {
                            Id = reminderId,
                            ContainerId = containerId,
                            Title = request.Title,
                            DueDate = request.DueDate,
                            Type = request.Type,
                            CreatedBy = userId
                        };
                        
                        var createdReminder = await reminderRepository.Create(createReminderModel, cancellationToken);
                        return createdReminder;
                    }
                    catch (ReminderException exception)
                    {
                        return new ReminderUnknownException(reminderId, exception);
                    }
                },
                () => Task.FromResult<Result<ReminderEntity, ReminderException>>(
                    new ContainerForReminderNotFoundException(reminderId))
            );
        }
    }