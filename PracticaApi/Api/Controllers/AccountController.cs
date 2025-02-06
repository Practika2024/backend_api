using Api.Dtos.Authentications;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Services;
using Application.Services.AuthenticationServices.RefreshTokenService;
using Application.Services.AuthenticationServices.SignInService;
using Application.Services.AuthenticationServices.SignUpService;
using Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ISignUpService _signUpService;
    private readonly ISignInService _signInService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AccountController(ISignUpService signUpService, ISignInService signInService, IRefreshTokenService refreshTokenService)
    {
        _signUpService = signUpService;
        _signInService = signInService;
        _refreshTokenService = refreshTokenService;
    }

    [HttpPost("signup")]
    public async Task<ActionResult<JwtVM>> SignUpAsync([FromBody] SignUpDto request, CancellationToken cancellationToken)
    {
        var result = await _signUpService.SignUpAsync(request.Email, request.Password, request.Name,request.Surname,request.Patronymic, cancellationToken);
        return result.Match<ActionResult<JwtVM>>(
            token => token,
            error => error.ToObjectResult()
        );
    }

    [HttpPost("signin")]
    public async Task<ActionResult<JwtVM>> SignInAsync([FromBody] SignInDto request, CancellationToken cancellationToken)
    {
        var result = await _signInService.SignInAsync(request.Email, request.Password, cancellationToken);
        return result.Match<ActionResult<JwtVM>>(
            token => token,
            error => error.ToObjectResult()
        );
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<JwtVM>> RefreshTokensAsync([FromBody] JwtVM model, CancellationToken cancellationToken)
    {
        var result = await _refreshTokenService.RefreshTokenAsync(model.AccessToken, model.RefreshToken, cancellationToken);
        return result.Match<ActionResult<JwtVM>>(
            token => token,
            error => error.ToObjectResult()
        );
    }
}