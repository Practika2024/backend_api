using Api.Dtos.Containers;
using Api.Modules.Errors;
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
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class ContainersController(ISender sender, IContainerQueries containerQueries, IMapper mapper) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await containerQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<ContainerDto>).ToList());
    }

    [HttpGet("get-by-id/{containerId:guid}")]
    public async Task<ActionResult<ContainerDto>> GetById([FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetById(containerId, cancellationToken);

        return entity.Match<ActionResult<ContainerDto>>(
            p => Ok(mapper.Map<ContainerDto>(p)),
            () => NotFound());
    }
    
    [HttpGet("get-by-fill-status/{isEmpty:bool}")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetContainersByFillStatus(
        [FromRoute] bool isEmpty,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByFillStatus(isEmpty, cancellationToken);
        return Ok(containers.Select(mapper.Map<ContainerDto>).ToList());
    }

    [HttpGet("get-by-product-type/{productTypeId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetContainersByProductType(
        [FromRoute] Guid productTypeId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByProductType(productTypeId, cancellationToken);
        return Ok(containers.Select(mapper.Map<ContainerDto>).ToList());
    }

    [HttpGet("get-by-product/{productId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetContainersByProduct(
        [FromRoute] Guid productId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetContainersByProduct(productId, cancellationToken);
        return Ok(containers.Select(mapper.Map<ContainerDto>).ToList());
    }

    [HttpGet("get-empty-by-last-product/{lastProductId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ContainerDto>>> GetEmptyContainersByLastProduct(
        [FromRoute] Guid lastProductId,
        CancellationToken cancellationToken)
    {
        var containers = await containerQueries.GetEmptyContainersByLastProduct(lastProductId, cancellationToken);
        return Ok(containers.Select(mapper.Map<ContainerDto>).ToList());
    }

    [HttpGet("get-unique-code-id/{uniqueCode}")]
    public async Task<ActionResult<ContainerDto>> GetById([FromRoute] string uniqueCode,
        CancellationToken cancellationToken)
    {
        var entity = await containerQueries.GetByUniqueCode(uniqueCode, cancellationToken);

        return entity.Match<ActionResult<ContainerDto>>(
            p => Ok(mapper.Map<ContainerDto>(p)),
            () => NotFound());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
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
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
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
            dto => Ok(mapper.Map<ContainerDto>(dto)),
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
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
            e => e.ToObjectResult());
    }
    
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
            e => e.ToObjectResult());
    }
    
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
            e => e.ToObjectResult());
    }
}