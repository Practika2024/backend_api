using Application.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using Application.Services.TokenService;
using Domain.RefreshTokens;
using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Application.Commands.Authentications.Commands;

public record RefreshTokenCommand : IRequest<Result<JwtModel, AuthenticationException>>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}

public class RefreshTokenCommandHandler(
    IJwtTokenService jwtTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository
)
    : IRequestHandler<RefreshTokenCommand, Result<JwtModel, AuthenticationException>>
{
    public async Task<Result<JwtModel, AuthenticationException>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var existingRefreshToken =
            await refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

        return await existingRefreshToken.Match(
            async rt => await RefreshToken(rt, request.AccessToken, cancellationToken),
            () => Task.FromResult<Result<JwtModel, AuthenticationException>>(
                new InvalidTokenException()));
    }

    private async Task<Result<JwtModel, AuthenticationException>> RefreshToken(RefreshTokenEntity storedToken,
        string accessToken, CancellationToken cancellationToken)
    {
        if (storedToken.IsUsed)
        {
            return await Task.FromResult<Result<JwtModel, AuthenticationException>>(new InvalidTokenException());
        }

        if (storedToken.ExpiredDate < DateTime.UtcNow)
        {
            return await Task.FromResult<Result<JwtModel, AuthenticationException>>(new TokenExpiredException());
        }

        var principals = jwtTokenService.GetPrincipals(accessToken);

        var accessTokenId = principals.Claims
            .Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        if (storedToken.JwtId != accessTokenId)
        {
            return await Task.FromResult<Result<JwtModel, AuthenticationException>>(new InvalidAccessTokenException());
        }

        var existingUser = await userRepository.GetById(storedToken.UserId, cancellationToken);

        return await existingUser.Match<Task<Result<JwtModel, AuthenticationException>>>(
            async u => await jwtTokenService.GenerateTokensAsync(u, cancellationToken),
            () => Task.FromResult<Result<JwtModel, AuthenticationException>>(
                new UserNorFoundException(storedToken.UserId)));
    }
}