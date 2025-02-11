﻿using Application.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Models.UserModels;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Domain.Authentications;
using Domain.Authentications.Users;
using MediatR;

namespace Application.Commands.Authentications.Commands;

public class SignUpCommand : IRequest<Result<JwtModel, AuthenticationException>>
{
    public required string Email { get; init; }
    public required string? Surname { get; init; }
    public required string? Patronymic { get; init; }
    public required string Password { get; init; }
    public required string? Name { get; init; }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IHashPasswordService hashPasswordService)
    : IRequestHandler<SignUpCommand, Result<JwtModel, AuthenticationException>>
{
    public async Task<Result<JwtModel, AuthenticationException>> Handle(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);

         return await existingUser.Match<Task<Result<JwtModel, AuthenticationException>>>(
            u => Task.FromResult<Result<JwtModel, AuthenticationException>>(
                new UserByThisEmailAlreadyExistsAuthenticationException(u.Id)),
            async () => await SignUp(request, cancellationToken)
        );
    }

    private async Task<Result<JwtModel, AuthenticationException>> SignUp(
        SignUpCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = UserId.New();
            var userModel = new CreateUserModel
            {
                Id = userId,
                Email = request.Email,
                Name = request.Name,
                Surname = request.Surname,
                Patronymic = request.Patronymic,
                PasswordHash = hashPasswordService.HashPassword(request.Password)
            };
            UserEntity userEntity = await userRepository.Create(userModel, cancellationToken);
            var addRoleModel = new AddRoleToUserModel
            {
                UserId = userEntity.Id,
                RoleId = AuthSettings.OperatorRole
            };
            var updatedUser = await userRepository.AddRole(addRoleModel, cancellationToken);
            var token = await jwtTokenService.GenerateTokensAsync(updatedUser, cancellationToken);

            return token;
        }
        catch (Exception exception)
        {
            return new AuthenticationUnknownException(UserId.Empty, exception);
        }
    }
}