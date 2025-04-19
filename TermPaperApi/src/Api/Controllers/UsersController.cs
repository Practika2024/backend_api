using Api.Dtos.Users;
using Api.Modules.Errors;
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
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class UsersController(ISender sender, IUserQueries userQueries, IMapper mapper) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);
        return Ok(entities.Select(mapper.Map<UserDto>).ToList());
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all-without-approval")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAllWithoutApproval(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAllWithoutApproval(cancellationToken);
        return Ok(entities.Select(mapper.Map<UserDto>).ToList());
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPatch("approve/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> ApproveUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new ApproveUserCommand() { UserId = userId };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            user => Ok(mapper.Map<UserDto>(user)),
            e => e.ToObjectResult());
    }
    
    [HttpGet("get-by-id/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(userId, cancellationToken);
        
        return entity.Match<ActionResult<UserDto>>(
            p => Ok(mapper.Map<UserDto>(p)),
            () => NotFound());
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("create")]
    public async Task<ActionResult<UserDto>> SignUpAsync(
        [FromBody] CreateUserDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreateUserCommand
        {
            Email = request.Email,
            Surname = request.Surname,
            Patronymic = request.Patronymic,
            Password = request.Password,
            Name = request.Name,
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => Ok(mapper.Map<UserDto>(u)),
            e => e.ToObjectResult()); 
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
            u => Ok(mapper.Map<UserDto>(u)),
            e => e.ToObjectResult()); 
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult()); 
    }
    
    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
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
    
        return result.Match<ActionResult<UserDto>>(
            u => Ok(mapper.Map<UserDto>(u)),
            e => e.ToObjectResult()); 
    }
    
    [HttpPatch("send-email-confirmation")]
    public async Task<ActionResult<UserDto>> EmailConfirmation(
        CancellationToken cancellationToken)
    {
        var command = new SendEmailConfirmationCommand
        {
        };
    
        var result = await sender.Send(command, cancellationToken);
    
        return result.Match<ActionResult<UserDto>>(
            u => Ok(mapper.Map<UserDto>(u)),
            e => e.ToObjectResult()); 
    }
}