using System.Net;
using System.Security.Claims;
using Application.Commands.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.TokenService;
using Domain.RefreshTokens;
using Domain.Users.Models;
using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.Commands.Authentications.Commands;

public record RefreshTokenCommand : IRequest<ServiceResponse>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}

public class RefreshTokenCommandHandler(
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository
)
    : IRequestHandler<RefreshTokenCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var existingRefreshToken =
            await refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

        return await existingRefreshToken.Match(
            async rt => await RefreshToken(rt, request.AccessToken, cancellationToken),
            () => Task.FromResult(
                ServiceResponse.GetResponse("Invalid token", false, null, HttpStatusCode.UpgradeRequired)
            ));
    }

    private async Task<ServiceResponse> RefreshToken(RefreshToken storedToken,
        string accessToken, CancellationToken cancellationToken)
    {
        if (storedToken.IsUsed)
        {
            return await Task.FromResult(ServiceResponse.GetResponse("Invalid token", false, null,
                HttpStatusCode.UpgradeRequired));
        }

        if (storedToken.ExpiredDate < DateTime.UtcNow)
        {
            return await Task.FromResult(ServiceResponse.GetResponse("Token has expired!", false, null,
                HttpStatusCode.UpgradeRequired));
        }

        ClaimsPrincipal principals;
        
        try
        {
            principals = jwtTokenService.GetPrincipals(accessToken);
        }
        catch (Exception e)
        {
            return await Task.FromResult(ServiceResponse.GetResponse("Invalid token", false, null,
                HttpStatusCode.UpgradeRequired));
        }
        

        var accessTokenId = principals.Claims
            .Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        if (storedToken.JwtId != accessTokenId)
        {
            return await Task.FromResult(ServiceResponse.GetResponse("Token has expired!", false, null,
                HttpStatusCode.UpgradeRequired));
        }

        var existingUser = await userRepository.GetById(storedToken.UserId, cancellationToken);

        return await existingUser.Match<Task<ServiceResponse>>(
            async u =>
            {
                var tokens = await jwtTokenService.GenerateTokensAsync(u, cancellationToken);
                return ServiceResponse.OkResponse("Users tokens", tokens);
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse($"User under id: {storedToken.UserId} was not found!")
                ));
    }
}