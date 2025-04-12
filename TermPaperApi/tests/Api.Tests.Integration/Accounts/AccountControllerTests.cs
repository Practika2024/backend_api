using System.Net;
using System.Net.Http.Json;
using Domain.Users.Models;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Accounts;

public class AccountControllerTests(IntegrationTestWebFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ShouldSignUp()
    {
        // Arrange
        var request = AccountData.SignUpRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signup", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadFromJsonAsync<JwtModel>();
        content.Should().NotBeNull();
        content!.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotSignUpWithInvalidEmail()
    {
        // Arrange
        var request = AccountData.SignUpWithInvalidEmailRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signup", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotSignUpWithoutPassword()
    {
        // Arrange
        var request = AccountData.SignUpWithOutPasswordRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signup", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task ShouldNotSignUpWithoutName()
    {
        // Arrange
        var request = AccountData.SignUpWithoutNameRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signup", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task ShouldSignIn()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var signInRequest = AccountData.SignInRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signin", signInRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadFromJsonAsync<JwtModel>();
        content.Should().NotBeNull();
        content!.Should().NotBeNull();
    }


    [Fact]
    public async Task ShouldNotSignInWithInvalidCredentials()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var request = AccountData.SignInWithInvalidCredentialsRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signin", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotSignInWithInvalidEmail()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var request = AccountData.SignInWithInvalidEmailRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signin", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldNotSignInWithoutPassword()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var request = AccountData.SignInWithoutPasswordRequest;

        // Act
        var response = await Client.PostAsJsonAsync("account/signin", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ShouldRefreshTokens()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var signInRequest = AccountData.SignInRequest;
        var signInResponse = await Client.PostAsJsonAsync("account/signin", signInRequest);
        var tokens = await signInResponse.Content.ReadFromJsonAsync<JwtModel>();

        var refreshRequest = new JwtModel
        {
            AccessToken = tokens!.AccessToken,
            RefreshToken = tokens.RefreshToken
        };

        // Act
        var response = await Client.PostAsJsonAsync("account/refresh-token", refreshRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var newTokens = await response.Content.ReadFromJsonAsync<JwtModel>();
        newTokens.Should().NotBeNull();
        newTokens!.AccessToken.Should().NotBe(tokens.AccessToken);
        newTokens.RefreshToken.Should().NotBe(tokens.RefreshToken);
    }

    [Fact]
    public async Task ShouldNotRefreshTokensWithInvalidRefreshToken()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var signInRequest = AccountData.SignInRequest;
        var signInResponse = await Client.PostAsJsonAsync("account/signin", signInRequest);
        var tokens = await signInResponse.Content.ReadFromJsonAsync<JwtModel>();

        var refreshRequest = new JwtModel
        {
            AccessToken = tokens!.AccessToken,
            RefreshToken = "invalid-refresh-token"
        };

        // Act
        var response = await Client.PostAsJsonAsync("account/refresh-token", refreshRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UpgradeRequired);
    }

    [Fact]
    public async Task ShouldNotRefreshTokensWithExpiredAccessToken()
    {
        // Arrange
        var signUpRequest = AccountData.SignUpForSignInRequest;
        await Client.PostAsJsonAsync("account/signup", signUpRequest);

        var signInRequest = AccountData.SignInRequest;
        var signInResponse = await Client.PostAsJsonAsync("account/signin", signInRequest);
        var tokens = await signInResponse.Content.ReadFromJsonAsync<JwtModel>();

        // Імітація дії з простроченим токеном (у реальному світі можна зробити через зміну часу в системі).
        var refreshRequest = new JwtModel
        {
            AccessToken = "expired-access-token",
            RefreshToken = tokens!.RefreshToken
        };

        // Act
        var response = await Client.PostAsJsonAsync("account/refresh-token", refreshRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldNotRefreshTokensWithoutTokens()
    {
        // Arrange
        var refreshRequest = new JwtModel
        {
            AccessToken = null,
            RefreshToken = null
        };

        // Act
        var response = await Client.PostAsJsonAsync("account/refresh-token", refreshRequest);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}