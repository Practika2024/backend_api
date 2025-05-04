using Api.Dtos.Authentications;
using Api.Modules.Errors;
using Application.Commands.Authentications.Commands;
using AutoMapper;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("account")]
[ApiController]
public class AccountController(ISender sender, IMapper mapper) : BaseController(mapper)
{
    private readonly IMapper _mapper = mapper;

    [HttpPost("signup")]
    public async Task<IActionResult> SignUpAsync(
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

        return GetResult(result);
    }
    
    [HttpPost("signin")]
    public async Task<IActionResult> SignUpAsync(
        [FromBody] SignInDto request,
        CancellationToken cancellationToken)
    {
        var input = new SignInCommand
        {
            Email = request.Email,
            Password = request.Password
        };
        
        var result = await sender.Send(input, cancellationToken);

        return GetResult(result);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokensAsync([FromBody] JwtModel model, CancellationToken cancellationToken)
    {
        var input = new RefreshTokenCommand()
        {
            AccessToken = model.AccessToken,
            RefreshToken = model.RefreshToken
        };
        
        var result = await sender.Send(input, cancellationToken);

        return GetResult(result);
    }
    
    [HttpPost("externalLogin")]
    public async Task<IActionResult> GoogleExternalLoginAsync([FromBody] ExternalLoginDto model, CancellationToken cancellationToken)
    {
        var command = new GoogleExternalLoginCommand { Model = _mapper.Map<ExternalLoginModel>(model) };
        var result = await sender.Send(command, cancellationToken);

        return GetResult(result);
    }
}