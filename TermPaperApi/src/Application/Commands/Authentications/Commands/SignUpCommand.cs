﻿using System.Net;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Application.Settings;
using Domain.Users;
using Domain.Users.Models;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands.Authentications.Commands;

public class SignUpCommand : IRequest<ServiceResponse>
{
    public required string Email { get; init; }
    public required string? Surname { get; init; }
    public required string? Patronymic { get; init; }
    public required string Password { get; init; }
    public required string? Name { get; init; }
}

public class SignUpUserCommandHandler(
    IUserRepository userRepository,
    IUserQueries userQueries,
    IJwtTokenService jwtTokenService,
    IHashPasswordService hashPasswordService)
    : IRequestHandler<SignUpCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);

        return await existingUser.Match<Task<ServiceResponse>>(
            u => Task.FromResult(
                ServiceResponse.BadRequestResponse("User with this email already exists")
                ),
            async () => await SignUp(request, cancellationToken)
        );
    }

    private async Task<ServiceResponse> SignUp(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = Guid.NewGuid();
            var isUsersNullOrEmpty = (await userQueries.GetAll(cancellationToken)).IsNullOrEmpty();
            var userModel = new CreateUserModel
            {
                Id = userId,
                Email = request.Email,
                Name = request.Name,
                Surname = request.Surname,
                Patronymic = request.Patronymic,
                PasswordHash = hashPasswordService.HashPassword(request.Password),
                RoleId = isUsersNullOrEmpty ? AuthSettings.AdminRole : AuthSettings.OperatorRole,
                CreatedBy = userId,
                IsApprovedByAdmin = isUsersNullOrEmpty ? true : null,
            };
            User userEntity = await userRepository.Create(userModel, cancellationToken);

            if (!userModel.IsApprovedByAdmin.HasValue)
            {
                return ServiceResponse.OkResponse("You don't have access token, please wait for admin approval");
            }

            var token = await jwtTokenService.GenerateTokensAsync(userEntity, cancellationToken);
            
            return ServiceResponse.OkResponse("Users tokens", token);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
        }
    }
}