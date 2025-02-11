using Api.Modules.Errors;
using Application.Commands.Authentications.Commands;
using Application.Dtos.Authentications;
using Application.Models.UserModels;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AccountEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("account");
        
        group.MapPost("signup", SignUpAsync);
        
        group.MapPost("signin", SignInAsync);
        
        group.MapPost("refresh-token", RefreshTokensAsync);
    }
    
    public static async Task<IResult> SignUpAsync(
        [FromBody] SignUpDto request,
        CancellationToken cancellationToken, ISender sender)
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

        return result.Match<IResult>(
            f => Results.Ok(f),
            e => e.ToIResult());

    }
    
    public static async Task<IResult> SignInAsync(
        [FromBody] SignInDto request,
        CancellationToken cancellationToken, ISender sender)
    {
        var input = new SignInCommand
        {
            Email = request.Email,
            Password = request.Password
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<IResult>(
            f => Results.Ok(f),
            e => e.ToIResult());
    }
    
    public static async Task<IResult> RefreshTokensAsync([FromBody] JwtModel model,
        CancellationToken cancellationToken, ISender sender)
    {
        var input = new RefreshTokenCommand()
        {
            AccessToken = model.AccessToken,
            RefreshToken = model.RefreshToken
        };
        
        var result = await sender.Send(input, cancellationToken);

        return result.Match<IResult>(
            f => Results.Ok(f),
            e => e.ToIResult());
    }
}