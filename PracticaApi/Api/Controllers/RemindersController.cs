using Api.Dtos.Reminders;
using Application.Commands.Reminders.Commands;
using Application.Common.Interfaces.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("reminders")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RemindersController(ISender sender, IReminderQueries reminderQueries, IMapper mapper) : BaseController
    {
        
        [HttpGet("get-all")]
        public async Task<ActionResult<IReadOnlyList<ReminderDto>>> GetAll(CancellationToken cancellationToken)
        {
            var entities = await reminderQueries.GetAll(cancellationToken);
            return Ok(entities.Select(mapper.Map<ReminderDto>).ToList());
        }
        
        [HttpGet("get-by-id/{reminderId:guid}")]
        public async Task<ActionResult<ReminderDto>> GetById([FromRoute] Guid reminderId,
            CancellationToken cancellationToken)
        {
            var entity = await reminderQueries.GetById(reminderId, cancellationToken);
            return entity.Match<ActionResult<ReminderDto>>(
                p => Ok(mapper.Map<ReminderDto>(p)),
                () => NotFound());
        }
        
        [HttpPost("add{containerId}")]
        // [Authorize(Roles = "Operator")]
        public async Task<ActionResult<ReminderDto>> AddReminderToContainer(
            [FromRoute] Guid containerId,
            [FromBody] AddReminderToContainerDto model,
            CancellationToken cancellationToken)
        {
            var command = new AddReminderToContainerCommand
            {
                ContainerId = containerId,
                Title = model.Title,
                DueDate = model.DueDate,
                Type = model.Type,
                CreatedBy = model.CreatedBy
            };

            var result = await sender.Send(command, cancellationToken);

            return result.Match<ActionResult<ReminderDto>>(
                dto => CreatedAtAction(nameof(GetById), new { reminderId = dto.Id }, dto),
                e => Problem(e.Message));
        }
    }
}