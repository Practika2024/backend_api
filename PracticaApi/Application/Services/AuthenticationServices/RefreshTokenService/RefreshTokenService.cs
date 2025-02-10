using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.RefreshTokens;

namespace Application.Services.AuthenticationServices.RefreshTokenService;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public RefreshTokenService(IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
    {
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<JwtVM, AuthenticationException>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var existingRefreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken, cancellationToken);

        return await existingRefreshToken.Match<Task<Result<JwtVM, AuthenticationException>>>(
            async rt => await VerifyRefreshToken(rt, accessToken, cancellationToken),
            () => Task.FromResult<Result<JwtVM, AuthenticationException>>(new InvalidTokenException())
        );
    }

    private async Task<Result<JwtVM, AuthenticationException>> VerifyRefreshToken(RefreshTokenEntity storedTokenEntity, string accessToken, CancellationToken cancellationToken)
    {
        if (storedTokenEntity.IsUsed || storedTokenEntity.ExpiredDate < DateTime.UtcNow)
        {
            return new InvalidTokenException();
        }

        var principals = _jwtTokenService.GetPrincipals(accessToken);
        var accessTokenId = principals.Claims.Single(c => c.Type == "jti").Value;

        if (storedTokenEntity.JwtId != accessTokenId)
        {
            return new InvalidAccessTokenException();
        }

        var existingUser = await _userRepository.GetById(storedTokenEntity.UserId, cancellationToken);

        return await existingUser.Match<Task<Result<JwtVM, AuthenticationException>>>(
            async user => await _jwtTokenService.GenerateTokensAsync(user, cancellationToken),
            () => Task.FromResult<Result<JwtVM, AuthenticationException>>(new UserNorFoundException(storedTokenEntity.UserId))
        );
    }
}