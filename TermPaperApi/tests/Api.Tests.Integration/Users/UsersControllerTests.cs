using System.Net;
using System.Net.Http.Json;
using Api.Dtos.Users;
using Application.Settings;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.Users;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Users;

public class UsersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly User _mainUser;

    public UsersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainUser = UsersData.MainUser();
    }

    [Fact]
    public async Task ShouldGetAllUsers()
    {
        // Act
        var response = await Client.GetAsync("users/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetUserById()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.GetAsync($"users/get-by-id/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(userId);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNonExistentUser()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"users/get-by-id/{nonExistentUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldCreateUser()
    {
        // Arrange
        var newUser = new CreateUserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            Name = "New",
            Surname = "User",
            Patronymic = "Test"
        };

        // Act
        var response = await Client.PostAsJsonAsync("users/create", newUser);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be(newUser.Email);
    }

    [Fact]
    public async Task ShouldUpdateUser()
    {
        // Arrange
        var newEmail = "updated@example.com";
        var newName = "Updated Name";

        var updateUserDto = new UpdateUserDto
        {
            Email = newEmail,
            Name = newName,
            Surname = "Updated Surname",
            Patronymic = "Updated Patronymic"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"users/update/{_mainUser.Id}", updateUserDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();

        var userFromDatabase = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == updatedUser!.Id);

        userFromDatabase.Should().NotBeNull();
        userFromDatabase!.Email.Should().Be(newEmail);
        userFromDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldUpdateUserRole()
    {
        // Arrange
        var newRoleId = AuthSettings.AdminRole;

        // Act
        var response = await Client.PutAsJsonAsync($"users/update-role/{_mainUser.Id}", newRoleId);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();

        var userFromDatabase = await Context.Users
            .FirstOrDefaultAsync(x => x.Id == updatedUser!.Id);

        userFromDatabase.Should().NotBeNull();
        userFromDatabase!.RoleId.Should().Be(newRoleId);
    }

    [Fact]
    public async Task ShouldDeleteUser()
    {
        // Arrange
        var userId = _mainUser.Id;

        // Act
        var response = await Client.DeleteAsync($"users/delete/{userId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public async Task InitializeAsync()
    {
        var userEntity = new UserEntity
        {
            Id = _mainUser.Id,
            Email = _mainUser.Email,
            Name = _mainUser.Name,
            Surname = _mainUser.Surname,
            Patronymic = _mainUser.Patronymic,
            PasswordHash = "hashedpassword",
            RoleId = "Operator",
            CreatedBy = UserId
        };
        await Context.Users.AddAsync(new UserEntity
            { Id = UserId, Email = "qwerty@gmail.com", PasswordHash = "fdsafdsafsad", RoleId = "Administrator" });

        await Context.Users.AddAuditableAsync(userEntity);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}