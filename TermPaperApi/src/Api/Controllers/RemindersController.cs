using Api.Dtos.Reminders;
using Application.Commands.Reminders.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("reminders")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class RemindersController(ISender sender, IReminderQueries reminderQueries, IMapper mapper) : BaseController
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await reminderQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Reminders list", entities.Select(mapper.Map<ReminderDto>)));
    }

    [HttpGet("get-by-id/{reminderId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid reminderId,
        CancellationToken cancellationToken)
    {
        var entity = await reminderQueries.GetById(reminderId, cancellationToken);
        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Reminder", mapper.Map<ReminderDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder not found")));
    }

    [HttpPost("add/{containerId}")]
    public async Task<IActionResult> AddReminderToContainer(
        [FromRoute] Guid containerId,
        [FromBody] AddReminderToContainerDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddReminderToContainerCommand
        {
            ContainerId = containerId,
            Title = model.Title,
            DueDate = model.DueDate,
            Type = model.Type
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [HttpPut("update/{reminderId:guid}")]
    public async Task<IActionResult> UpdateReminder(
        [FromRoute] Guid reminderId,
        [FromBody] UpdateReminderDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReminderCommand
        {
            Id = reminderId,
            Title = model.Title,
            DueDate = model.DueDate,
            Type = model.Type
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [HttpDelete("delete/{reminderId:guid}")]
    public async Task<IActionResult> DeleteReminder(
        [FromRoute] Guid reminderId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteReminderCommand
        {
            Id = reminderId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}