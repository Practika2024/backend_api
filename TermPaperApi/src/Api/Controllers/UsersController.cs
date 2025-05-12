using Api.Dtos;
using Api.Dtos.Users;
using Application.Commands.Users.Commands;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Services.PaginationService;
using Application.Settings;
using AutoMapper;
using Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = $"{AuthSettings.AdminRole}, {AuthSettings.OperatorRole}")]
public class UsersController(
    ISender sender,
    IUserQueries userQueries,
    IMapper mapper) : BaseController(mapper)
{
    private readonly IMapper _mapper = mapper;

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] PaginationDto pagination, CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAll(cancellationToken);
        if (pagination.Page is null && pagination.PageSize is null)
            return GetResult(ServiceResponse.OkResponse("Users list", entities.Select(_mapper.Map<UserDto>)));
        
        var response = PaginationService.GetEntitiesWithPagination(pagination.Page, pagination.PageSize,
            entities.ToList());

        return GetResult<EntitiesListModel<UserDto>>(response);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all-without-approval")]
    public async Task<IActionResult> GetAllWithoutApproval(CancellationToken cancellationToken)
    {
        var entities = await userQueries.GetAllWithoutApproval(cancellationToken);
        return GetResult(ServiceResponse.OkResponse("Users without approval", entities.Select(_mapper.Map<UserDto>)));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPatch("approve/{userId:guid}")]
    public async Task<IActionResult> ApproveUser([FromRoute] Guid userId, [FromQuery] bool isUserApproved = true,
        CancellationToken cancellationToken = default)
    {
        var command = new ApproveUserCommand() { UserId = userId, IsUserApproved = isUserApproved };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-by-id/{userId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await userQueries.GetById(userId, cancellationToken);

        return entity.Match<IActionResult>(
            p => GetResult(ServiceResponse.OkResponse("User", _mapper.Map<UserDto>(p))),
            () => GetResult(ServiceResponse.NotFoundResponse("User not found")));
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPost("create")]
    public async Task<IActionResult> SignUpAsync(
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

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update-role/{userId:guid}")]
    public async Task<IActionResult> UpdateRoles(
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

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { UserId = userId };
        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpPut("update/{userId:guid}")]
    public async Task<IActionResult> UpdateUser(
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

        return GetResult(result);
    }

    [HttpPost("send-email-confirmation")]
    public async Task<IActionResult> EmailConfirmation(
        CancellationToken cancellationToken)
    {
        var command = new SendEmailConfirmationCommand
        {
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }

    [HttpGet("verify-email", Name = "VerifyEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail(
        Guid token,
        CancellationToken cancellationToken)
    {
        var command = new VerificationEmailCommand()
        {
            TokenId = token
        };

        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}