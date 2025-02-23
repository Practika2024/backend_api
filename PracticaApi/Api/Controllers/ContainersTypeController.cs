using Api.Dtos.ContainersType;
using Application.Commands.ContainersType.Commands;
using Application.Common.Interfaces.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
//[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
[ApiController]
public class ContainersTypeController(ISender sender, IContainerTypeQueries containerTypeQueries, IMapper mapper) : BaseController
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ContainerTypeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await containerTypeQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ContainerTypeDto>).ToList());
    }
    
    [HttpPost("add")]
    public async Task<ActionResult<ContainerTypeDto>> AddContainerType(
        [FromBody] CreateUpdateContainerTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddContainerTypeCommand()
        {
            Name = model.Name,
            CreatedBy = GetUserId()!.Value
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerTypeDto>>(
            dto => mapper.Map<ContainerTypeDto>(dto),
            e => Problem(e.Message));
    }
    
    [HttpPut("update/{containerId:guid}")]
    public async Task<ActionResult<ContainerTypeDto>> UpdateContainerType(
        [FromRoute] Guid containerId,
        [FromBody] CreateUpdateContainerTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContainerTypeCommand
        {
            Id = containerId,
            Name = model.Name,
            ModifiedBy = GetUserId()!.Value,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerTypeDto>>(
            dto => Ok(mapper.Map<ContainerTypeDto>(dto)),
            e => Problem(e.Message));
    }
    
    [HttpDelete("delete/{containerId:guid}")]
    public async Task<ActionResult<ContainerTypeDto>> DeleteContainerType(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContainerTypeCommand
        {
            Id = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerTypeDto>>(
            dto => Ok(mapper.Map<ContainerTypeDto>(dto)),
            e => Problem(e.Message));
    }
}