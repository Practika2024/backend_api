using Api.Dtos.Users;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Services.UserServices.DeleteUserService;
using Application.Services.UserServices.UpdateUserService;
using Application.Services.UserServices.UploadUserImageService;
using Application.ViewModels;
using Domain.Authentications;
using Domain.Authentications.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("users")]
[ApiController]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsersController : ControllerBase
{
    private readonly IUserQueries _userQueries;
    private readonly IDeleteUserService _deleteUserService;
    private readonly IUpdateUserService _updateUserService;
    private readonly IUploadUserImageService _uploadUserImageService;

    public UsersController(
        IUserQueries userQueries,
        IDeleteUserService deleteUserService,
        IUpdateUserService updateUserService,
        IUploadUserImageService uploadUserImageService)
    {
        _userQueries = userQueries;
        _deleteUserService = deleteUserService;
        _updateUserService = updateUserService;
        _uploadUserImageService = uploadUserImageService;
    }
    
    // [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpGet("get-all")]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await _userQueries.GetAll(cancellationToken);

        return entities.Select(UserDto.FromDomainModel).ToList();
    }
    
    [Authorize(Roles = $"{AuthSettings.AdminRole},{AuthSettings.OperatorRole}")]
    [HttpGet("get-by-id/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Get([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var entity = await _userQueries.GetById(new UserId(userId), cancellationToken);

        return entity.Match<ActionResult<UserDto>>(
            p => UserDto.FromDomainModel(p),
            () => NotFound());
    }

    [Authorize(Roles = AuthSettings.AdminRole)]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var result = await _deleteUserService.DeleteUserAsync(userId, cancellationToken);
        return result.Match<ActionResult<UserDto>>(
            c => UserDto.FromDomainModel(c),
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPut("update/{userId:guid}")]
    public async Task<ActionResult<JwtVM>> UpdateUser([FromRoute] Guid userId, [FromBody] UpdateUserVM user, CancellationToken cancellationToken)
    {
        var result = await _updateUserService.UpdateUserAsync(userId, user.Email, user.UserName, cancellationToken);
        return result.Match<ActionResult<JwtVM>>(
            r => r,
            e => e.ToObjectResult());
    }

    [Authorize(Roles = AuthSettings.OperatorRole)]
    [HttpPut("image/{userId}")]
    public async Task<ActionResult<JwtVM>> Upload([FromRoute] Guid userId, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var result = await _uploadUserImageService.UploadUserImageAsync(userId, imageFile, cancellationToken);
        return result.Match<ActionResult<JwtVM>>(
            r => r,
            e => e.ToObjectResult());
    }
    
    
}