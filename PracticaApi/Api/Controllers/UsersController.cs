using Application.Commands.Users.Commands;
using Application.Common.Interfaces.Queries;
using Application.Dtos.Users;
using Application.Models.UserModels;
using Domain.Authentications;
using Domain.Authentications.Users;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController(ISender sender, IUserQueries userQueries) : ControllerBase
{
    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);

        return entities.Select(UserDto.FromDomainModel).ToList();
    }
    
    // [Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-by-id/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            p => UserDto.FromDomainModel(p),
            () => NotFound());
    }
    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update-roles/{userId:guid}")]
    public async Task<ActionResult> UpdateRoles(
        [FromRoute] Guid userId,
        [FromBody] List<string> roles,
        CancellationToken cancellationToken)
    {
        var command = new ChangeRolesForUserCommand
        {
            UserId = userId,
            Roles = roles
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
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
    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<JwtModel>> UpdateUser(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserDto model,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand
        {
            UserId = userId,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname,
            Patronymic = model.Patronymic
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<JwtModel>>(
            jwt => Ok(jwt),
            e => Problem(e.Message));
    }
}