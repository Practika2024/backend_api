using Api.Dtos.Containers;
using Application.Commands.Containers.Commands;
using Application.Common.Interfaces.Queries;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("containers-type")]
[ApiController]
//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ContainersTypeController(ISender sender, IContainerQueries containerQueries, IMapper mapper) : ControllerBase
{

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
            UserId = model.CreatedBy,
            TypeId = model.TypeId
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ContainerDto>>(
            dto => mapper.Map<ContainerDto>(dto),
            e => Problem(e.Message));
    }
}