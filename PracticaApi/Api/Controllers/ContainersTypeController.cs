using Api.Dtos.ContainersType;
using Application.Commands.ContainersType.Commands;
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
public class ContainersTypeController(ISender sender, IMapper mapper) : BaseController
{
    [HttpPost("add")]
    public async Task<ActionResult<ContainerTypeDto>> AddContainer(
        [FromBody] ContainerTypeDto model,
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
}