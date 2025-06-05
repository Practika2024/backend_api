using System.Net;
using System.Net.Http.Json;
using Api.Dtos.ReminderType;
using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.ReminderTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class ReminderTypeControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly ReminderType _mainReminderType;

    public ReminderTypeControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainReminderType = ReminderTypeData.MainReminderType;
    }

    [Fact]
    public async Task ShouldCreateReminderType()
    {
        // Arrange
        var reminderTypeName = "New Test Reminder Type";
        var request = new CreateUpdateReminderTypeDto { Name = reminderTypeName };

        // Act
        var response = await Client.PostAsJsonAsync("reminders-type/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var reminderTypeFromResponse = await JsonHelper.GetPayloadAsync<ReminderTypeDto>(response);
        var reminderTypeId = reminderTypeFromResponse.Id!.Value;

        var reminderTypeFromDatabase = await Context.ReminderTypes.FirstOrDefaultAsync(x => x.Id == reminderTypeId);

        reminderTypeFromDatabase.Should().NotBeNull();
        reminderTypeFromDatabase!.Name.Should().Be(reminderTypeName);
    }

    [Fact]
    public async Task ShouldUpdateReminderType()
    {
        var newName = "Updated Reminder Type";
        var request = new CreateUpdateReminderTypeDto { Name = newName };

        var response = await Client.PutAsJsonAsync($"reminders-type/update/{_mainReminderType.Id}", request);

        response.IsSuccessStatusCode.Should().BeTrue();
        
        var reminders = await JsonHelper.GetPayloadAsync<ReminderTypeDto>(response);

        var reminderTypeFromDatabase = await Context.ReminderTypes.FirstOrDefaultAsync(x => x.Id == _mainReminderType.Id);
        reminderTypeFromDatabase.Should().NotBeNull();
        reminderTypeFromDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldNotUpdateReminderTypeBecauseNotFound()
    {
        var request = new CreateUpdateReminderTypeDto { Name = "Non-Existent Reminder" };

        var response = await Client.PutAsJsonAsync($"reminders-type/update/{Guid.NewGuid()}", request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteReminderType()
    {
        var reminderId = _mainReminderType.Id;

        var response = await Client.DeleteAsync($"reminders-type/delete/{reminderId}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var reminderFromDatabase = await Context.ReminderTypes.FirstOrDefaultAsync(x => x.Id == reminderId);
        reminderFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteReminderTypeBecauseNotFound()
    {
        var response = await Client.DeleteAsync($"reminders-type/delete/{Guid.NewGuid()}");

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldGetAllReminderTypes()
    {
        var response = await Client.GetAsync("reminders-type/get-all");

        response.IsSuccessStatusCode.Should().BeTrue();

        var reminders = await JsonHelper.GetPayloadAsync<List<ReminderTypeDto>>(response);
        reminders.Should().NotBeEmpty();
    }

    public async Task InitializeAsync()
    {
        var reminderEntity = new ReminderTypeEntity
        {
            Name = _mainReminderType.Name,
            CreatedBy = UserId
        };

        await Context.Users.AddAsync(new UserEntity
            { Id = UserId, Email = "qwerty@gmail.com", PasswordHash = "fdsafdsafsad", RoleId = "Administrator" });
        await Context.ReminderTypes.AddAuditableAsync(reminderEntity);
        
        await SaveChangesAsync();
        
        var firstReminderType = await Context.ReminderTypes.AsNoTracking().FirstOrDefaultAsync();
        _mainReminderType.Id = firstReminderType!.Id;
    }

    public async Task DisposeAsync()
    {
        Context.ReminderTypes.RemoveRange(Context.ReminderTypes);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}