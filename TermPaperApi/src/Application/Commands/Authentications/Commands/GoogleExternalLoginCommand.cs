using System.Net;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Application.Settings;
using Domain.Users;
using Domain.Users.Models;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Optional.Unsafe;

namespace Application.Commands.Authentications.Commands;

public record GoogleExternalLoginCommand : IRequest<ServiceResponse>
{
    public required ExternalLoginModel Model { get; init; }
}

public class GoogleExternalLoginCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IHashPasswordService hashPasswordService,
    IUserQueries userQueries)
    : IRequestHandler<GoogleExternalLoginCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(GoogleExternalLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Model == null || string.IsNullOrEmpty(request.Model.Token))
                return ServiceResponse.BadRequestResponse("Google token not sent");

            var payload = await jwtTokenService.VerifyGoogleToken(request.Model);

            if (payload is null)
                return ServiceResponse.BadRequestResponse("Invalid Google token");

            var info = new UserLoginInfo(request.Model.Provider, payload.Subject, request.Model.Provider);

            var isUsersNullOrEmpty = (await userQueries.GetAll(cancellationToken)).IsNullOrEmpty();

            var user = await FindOrCreateUserAsync(payload, info, isUsersNullOrEmpty, cancellationToken);

            if (user is null)
                return ServiceResponse.BadRequestResponse("Failed to add Google login");

            if (!user.IsApprovedByAdmin.HasValue)
            {
                return ServiceResponse.OkResponse("You don't have access token, please wait for admin approval");
            }

            if (user.IsApprovedByAdmin == false)
            {
                return ServiceResponse.GetResponse("Your approval has been rejected", false, null,
                    HttpStatusCode.Forbidden);
            }

            var tokens = await jwtTokenService.GenerateTokensAsync(user, cancellationToken);
            return ServiceResponse.OkResponse("Users tokens", tokens);
        }
        catch (Exception ex)
        {
            return ServiceResponse.InternalServerErrorResponse(ex.Message);
        }
    }

    private async Task<User?> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload,
        UserLoginInfo info, bool isUsersNullOrEmpty, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByLoginAsync(info.LoginProvider, info.ProviderKey, cancellationToken);
        if (user != null)
            return user;

        user = (await userRepository.SearchByEmail(payload.Email, cancellationToken)).ValueOrDefault();
        if (user == null)
        {
            user = await CreateUserAsync(payload, isUsersNullOrEmpty, cancellationToken);
        }

        var loginResult = await userRepository.AddLoginAsync(user, info, cancellationToken);
        return loginResult.Succeeded ? user : null;
    }

    private async Task<User> CreateUserAsync(GoogleJsonWebSignature.Payload payload,
        bool isUsersNullOrEmpty,
        CancellationToken cancellationToken)
    {
        var userId = Guid.NewGuid();
        var randomPassword = GenerateRandomPassword();

        var (name, patronymic) = SplitFullName(payload.Name);

        var userModel = new CreateUserModel
        {
            Id = userId,
            Email = payload.Email,
            Name = name,
            Surname = payload.FamilyName,
            Patronymic = patronymic,
            RoleId = isUsersNullOrEmpty
                ? AuthSettings.AdminRole
                : AuthSettings.OperatorRole,
            PasswordHash = hashPasswordService.HashPassword(randomPassword),
            CreatedBy = userId,
            IsApprovedByAdmin = isUsersNullOrEmpty ? true : null
        };

        return await userRepository.Create(userModel, cancellationToken);
    }

    private (string? name, string? patronymic) SplitFullName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return (null, null);

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = parts.ElementAtOrDefault(0);
        var patronymic = parts.ElementAtOrDefault(1);

        return (name, patronymic);
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}