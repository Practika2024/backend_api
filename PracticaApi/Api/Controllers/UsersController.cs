using Api.Dtos.Users;
using Application.Commands.Users.Commands;
using Application.Common.Interfaces.Queries;
using Application.Settings;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = AuthSettings.AdminRole)]
public class UsersController(ISender sender, IUserQueries userQueries, IMapper mapper) : ControllerBase
{
    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);

        return entities.Select(mapper.Map<UserDto>).ToList();
    }
    
    // [Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-by-id/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(userId, cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            p => mapper.Map<UserDto>(p),
            () => NotFound());
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update-role/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateRoles(
        [FromRoute] Guid userId,
        [FromBody] string rolesId,
        CancellationToken cancellationToken)
    {
        var command = new ChangeRoleForUserCommand
        {
            UserId = userId,
            RoleId = rolesId
        };
    
        var result = await sender.Send(command, cancellationToken);
    
        return result.Match<ActionResult<UserDto>>(
            u => mapper.Map<UserDto>(u),
            e => Problem(e.Message));
    }

    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            e => Problem(e.Message));
    }

    // [Authorize(Roles = AuthSettings.OperatorRole)]
    // [HttpPut("update/{userId:guid}")]
    // public async Task<ActionResult<JwtModel>> UpdateUser(
    //     [FromRoute] Guid userId,
    //     [FromBody] UpdateUserDto model,
    //     CancellationToken cancellationToken)
    // {
    //     var command = new UpdateUserCommand
    //     {
    //         UserId = userId,
    //         Email = model.Email,
    //         Name = model.Name,
    //         Surname = model.Surname,
    //         Patronymic = model.Patronymic
    //     };
    //
    //     var result = await sender.Send(command, cancellationToken);
    //
    //     return result.Match<ActionResult<JwtModel>>(
    //         jwt => Ok(jwt),
    //         e => Problem(e.Message));
    // }
}