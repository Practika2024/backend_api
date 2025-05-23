﻿using System.Net;
using Application.Commands.Authentications.Exceptions;
using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Authentications.Commands;

public class SignInCommand : IRequest<ServiceResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class SignInCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IHashPasswordService hashPasswordService)
    : IRequestHandler<SignInCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);

        return await existingUser.Match(
            async u => await SignIn(u, request.Password, cancellationToken),
            () => Task.FromResult(
                ServiceResponse.BadRequestResponse("Email or password are incorrect")));
    }

    private async Task<ServiceResponse> SignIn(
        User user,
        string password,
        CancellationToken cancellationToken)
    {
        if (!user.IsApprovedByAdmin.HasValue)
        {
            return ServiceResponse.GetResponse("You don't have access token, please wait for admin approval", false, null, HttpStatusCode.OK);
        }

        if (!user.IsApprovedByAdmin.Value)
        {
            return ServiceResponse.ForbiddenResponse("Your approval has been rejected");
        }
        
        string storedHash = user.PasswordHash;

        if (!hashPasswordService.VerifyPassword(password, storedHash))
        {
            return ServiceResponse.BadRequestResponse("Email or password are incorrect");
        }

        try
        {
            var token = await jwtTokenService.GenerateTokensAsync(user, cancellationToken);
            return ServiceResponse.OkResponse("Users tokens", token);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
        }
    }
}