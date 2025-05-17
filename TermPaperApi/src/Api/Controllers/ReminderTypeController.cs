using Api.Dtos;
using Api.Dtos.ReminderType;
using Application.Commands.ReminderType.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Services.PaginationService;
using Application.Settings;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("reminders-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
[ApiController]
public class RemindersTypeController(
    ISender sender,
    IReminderTypeQueries reminderTypeQueries,
    IMapper mapper)
    : BaseController(mapper)
{
    private readonly IMapper _mapper = mapper;

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination, CancellationToken cancellationToken)
    {
        var entities = await reminderTypeQueries.GetAll(cancellationToken);
        if (pagination.Page is null && pagination.PageSize is null)
            return GetResult(ServiceResponse
                .OkResponse("Reminders types list", entities.Select(_mapper.Map<ReminderTypeDto>).ToList()));
        
        var response = PaginationService.GetEntitiesWithPagination(pagination.Page, pagination.PageSize,
            entities.ToList());

        return GetResult<EntitiesListModel<ReminderTypeDto>>(response);
    }

    [HttpGet("get-by-id/{reminderTypeId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int reminderTypeId,
        CancellationToken cancellationToken)
    {
        var entity = await reminderTypeQueries.GetById(reminderTypeId, cancellationToken);

        return entity.Match(
            p => GetResult(ServiceResponse.OkResponse("Reminder type", _mapper.Map<ReminderTypeDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Reminder type not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<IActionResult> AddReminderType(
        [FromBody] CreateUpdateReminderTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddReminderTypeCommand()
        {
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{reminderId:int}")]
    public async Task<IActionResult> UpdateReminderType(
        [FromRoute] int reminderId,
        [FromBody] CreateUpdateReminderTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReminderTypeCommand
        {
            Id = reminderId,
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{reminderId:int}")]
    public async Task<IActionResult> DeleteReminderType(
        [FromRoute] int reminderId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteReminderTypeCommand
        {
            Id = reminderId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}