using System.Security.Claims;
using Api.Dtos.Containers;
using Application.Commands.Containers.Commands;
using Application.Common.Interfaces.Queries;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ContainersController(ISender sender, IContainerQueries containerQueries, IMapper mapper) : ControllerBase
{
    //[Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await containerQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ContainerDto>).ToList());
    }

    //[Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-by-id/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> GetById([FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetById(containerId, cancellationToken);
        
        return entity.Match<ActionResult<ContainerDto>>(
            p => Ok(mapper.Map<ContainerDto>(p)),
            () => NotFound());
    }
    
    //[Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-unique-code-id/{uniqueCode}")]
    public async Task<ActionResult<ContainerDto>> GetById([FromRoute] string uniqueCode,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetByUniqueCode(uniqueCode, cancellationToken);
        
        return entity.Match<ActionResult<ContainerDto>>(
            p => Ok(mapper.Map<ContainerDto>(p)),
            () => NotFound());
    }
    
    //[Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPost("add")]
    public async Task<ActionResult<ContainerDto>> AddContainer(
        [FromBody] CreateContainerDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddContainerCommand
        {
            Name = model.Name,
            Volume = model.Volume,
            Notes = model.Notes,
            TypeId = model.TypeId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => mapper.Map<ContainerDto>(dto),
            e => Problem(e.Message));
    }

    //[Authorize(Roles = "Operator")]
    [HttpPut("update/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> UpdateContainer(
        [FromRoute] Guid containerId,
        [FromBody] UpdateContainerDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContainerCommand
        {
            Id = containerId,
            Name = model.Name,
            Notes = model.Notes,
            Volume = model.Volume,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => Ok(dto),
            e => Problem(e.Message));
    }

    //[Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpDelete("delete/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> DeleteContainer(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContainerCommand
        {
            Id = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => Ok(mapper.Map<ContainerDto>(dto)),
            e => Problem(e.Message));
    }

    //[Authorize(Roles = "Operator")]
    [HttpPut("set-content/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> SetContainerContent(
        [FromRoute] Guid containerId,
        [FromBody] SetContainerContentDto model,
        CancellationToken cancellationToken)
    {
        var command = new SetContainerContentCommand
        {
            ContainerId = containerId,
            ProductId = model.ProductId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => Ok(mapper.Map<ContainerDto>(dto)),
            e => Problem(e.Message));
    }

    // Clear container content (Operator role only)
    //[Authorize(Roles = "Operator")]
    [HttpPut("clear-content/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> ClearContainerContent(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new ClearContainerContentCommand
        {
            ContainerId = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => Ok(mapper.Map<ContainerDto>(dto)),
            e => Problem(e.Message));
    }
}