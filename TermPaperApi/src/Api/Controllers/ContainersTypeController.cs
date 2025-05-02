using Api.Dtos.ContainersType;
using Api.Modules.Errors;
using Application.Commands.ContainersType.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
[ApiController]
public class ContainersTypeController(ISender sender, IContainerTypeQueries containerTypeQueries, IMapper mapper)
    : BaseController
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var entities = await containerTypeQueries.GetAll(cancellationToken);
        
        return GetResult(ServiceResponse.OkResponse("Containers types list",
            entities.Select(mapper.Map<ContainerTypeDto>)));
    }

    [HttpGet("get-by-id/{containerTypeId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid containerTypeId,
        CancellationToken cancellationToken)
    {
        var entity = await containerTypeQueries.GetById(containerTypeId, cancellationToken);

        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("Container type", mapper.Map<ContainerTypeDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("Container type not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("add")]
    public async Task<IActionResult> AddContainerType(
        [FromBody] CreateUpdateContainerTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new AddContainerTypeCommand()
        {
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{containerId:guid}")]
    public async Task<IActionResult> UpdateContainerType(
        [FromRoute] Guid containerId,
        [FromBody] CreateUpdateContainerTypeDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateContainerTypeCommand
        {
            Id = containerId,
            Name = model.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{containerId:guid}")]
    public async Task<IActionResult> DeleteContainerType(
        [FromRoute] Guid containerId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteContainerTypeCommand
        {
            Id = containerId
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}