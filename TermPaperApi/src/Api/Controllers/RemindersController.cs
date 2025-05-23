using Api.Dtos.Reminders;
using Application.Commands.Reminders.Commands;
using Application.Common.Interfaces;
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
public class RemindersController(
    ISender sender,
    IReminderQueries reminderQueries,
    IMapper mapper,
    IUserProvider userProvider) : BaseController(mapper)
{
    private readonly IMapper _mapper = mapper;

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await reminderQueries.GetAll(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Reminders list", entities.Select(_mapper.Map<ReminderDto>)));
    }

    [HttpGet("get-by-id/{reminderId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid reminderId,
        CancellationToken cancellationToken)
    {
        var entity = await reminderQueries.GetById(reminderId, cancellationToken);
        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Reminder", _mapper.Map<ReminderDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder not found")));
    }

    [HttpGet("get-all-by-user")]
    public async Task<IActionResult> GetByUser(CancellationToken cancellationToken)
    {
        var entity = await reminderQueries.GetAllByUser(userProvider.GetUserId(), cancellationToken);
        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("All reminders", _mapper.Map<IReadOnlyList<ReminderDto>>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder not found")));
    }

    [HttpGet("get-not-completed-by-user")]
    public async Task<IActionResult> GetNotCompletedByUser(CancellationToken cancellationToken)
    {
        var allEntities = await reminderQueries.GetAllByUser(userProvider.GetUserId(), cancellationToken);
        
        return allEntities.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Not completed reminders",
                _mapper.Map<IReadOnlyList<ReminderDto>>(p.Where(r => r.DueDate > DateTime.UtcNow)))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder not found")));
    }
    
    [HttpGet("get-all-completed-by-user")]
    public async Task<IActionResult> GetAllCompletedByUser(CancellationToken cancellationToken)
    {
        var allEntities = await reminderQueries.GetAllCompletedByUser(userProvider.GetUserId(), cancellationToken);
        
        return allEntities.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Not completed reminders",
                _mapper.Map<IReadOnlyList<ReminderDto>>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder not found")));
    }
    
    [HttpGet("get-not-viewed-by-user")]
    public async Task<IActionResult> GetNotViewedByUser(CancellationToken cancellationToken)
    {
        var allEntities = await reminderQueries.GetAllByUser(userProvider.GetUserId(), cancellationToken);
        
        return allEntities.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Not completed reminders",
                _mapper.Map<IReadOnlyList<ReminderDto>>(p.Where(r => r.DueDate < DateTime.UtcNow && !r.IsViewed)))),
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
            TypeId = model.Type
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
            TypeId = model.Type,
            ContainerId = model.ContainerId
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