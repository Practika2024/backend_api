using Api.Dtos.Authentications;
using Api.Modules.Errors;
using Application.Commands.Authentications.Commands;
using AutoMapper;
using Domain.UserModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("account")]
[ApiController]
public class AccountController(ISender sender, IMapper mapper) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<ActionResult<JwtModel>> SignUpAsync(
        [FromBody] SignUpDto request,
        CancellationToken cancellationToken)
    {
        var input = new SignUpCommand
        {
            Email = request.Email,
            Surname = request.Surname,
            Patronymic = request.Patronymic,
            Password = request.Password,
            Name = request.Name,
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<JwtModel>>(
            f => f,
            e => e.ToObjectResult());
    }
    
    [HttpPost("signin")]
    public async Task<ActionResult<JwtModel>> SignUpAsync(
        [FromBody] SignInDto request,
        CancellationToken cancellationToken)
    {
        var input = new SignInCommand
        {
            Email = request.Email,
            Password = request.Password
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<JwtModel>>(
            f => f,
            e => e.ToObjectResult());
    }
    
    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtModel>> RefreshTokensAsync([FromBody] JwtModel model, CancellationToken cancellationToken)
    {
        var input = new RefreshTokenCommand()
        {
            AccessToken = model.AccessToken,
            RefreshToken = model.RefreshToken
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<JwtModel>>(
            f => f,
            e => e.ToObjectResult());
    }
    
    [HttpPost("GoogleExternalLogin")]
    public async Task<ActionResult<JwtModel>> GoogleExternalLoginAsync([FromBody] ExternalLoginDto model, CancellationToken cancellationToken)
    {
        var command = new GoogleExternalLoginCommand { Model = mapper.Map<ExternalLoginModel>(model) };
        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<JwtModel>>(
            f => f,
            e => e.ToObjectResult()); 
    }
}