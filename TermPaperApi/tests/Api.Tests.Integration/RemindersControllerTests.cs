using System.Net;
using System.Net.Http.Json;
using Api.Dtos.Reminders;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Reminders;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.Containers;
using Domain.ContainerTypes;
using Domain.Reminders;
using Domain.ReminderTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class RemindersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Reminder _mainReminder;
    private readonly ReminderType _mainReminderType = ReminderTypeData.SecondReminderType;
    private readonly Container _containersData;
    private readonly ContainerType _containerTypeData = ContainerTypeData.MainContainerType;

    public RemindersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _containersData = ContainersData.MainContainer(_containerTypeData.Id)!;
        _mainReminder = ReminderData.MainReminder(_mainReminderType.Id, _containersData.Id);
    }

    [Fact]
    public async Task ShouldAddReminderToContainer()
    {
        // Arrange
        var addReminderDto = new AddReminderToContainerDto
        {
            Title = "New Test Reminder",
            DueDate = DateTime.UtcNow.AddDays(1),
            Type = _mainReminderType.Id
        };

        // Act
        var response = await Client.PostAsJsonAsync($"reminders/add/{_containersData.Id}", addReminderDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var createdReminder = await JsonHelper.GetPayloadAsync<ReminderDto>(response);
        createdReminder.Should().NotBeNull();
        createdReminder!.Title.Should().Be(addReminderDto.Title);
        createdReminder.DueDate.Should().Be(addReminderDto.DueDate);
        createdReminder.TypeId.Should().Be(addReminderDto.Type);
        createdReminder.ContainerId.Should().Be(_containersData.Id);
    }

    [Fact]
    public async Task ShouldGetAllReminders()
    {
        // Act
        var response = await Client.GetAsync("reminders/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminders = await JsonHelper.GetPayloadAsync<List<ReminderDto>>(response);
        reminders.Should().NotBeNull();
        reminders!.Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task ShouldGetReminderById()
    {
        // Arrange
        var reminderId = _mainReminder.Id;

        // Act
        var response = await Client.GetAsync($"reminders/get-by-id/{reminderId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminder = await JsonHelper.GetPayloadAsync<ReminderDto>(response);
        reminder.Should().NotBeNull();
        reminder!.Id.Should().Be(reminderId);
        reminder.Title.Should().Be(_mainReminder.Title);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNonExistentReminder()
    {
        // Arrange
        var nonExistentReminderId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"reminders/get-by-id/{nonExistentReminderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldGetAllByUser()
    {
        // Act
        var response = await Client.GetAsync("reminders/get-all-by-user");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminders = await JsonHelper.GetPayloadAsync<List<ReminderDto>>(response);
        reminders.Should().NotBeNull();
        reminders!.Should().Contain(r => r.Id == _mainReminder.Id);
    }

    [Fact]
    public async Task ShouldGetNotCompletedByUser()
    {
        // Act
        var response = await Client.GetAsync("reminders/get-not-completed-by-user");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminders = await JsonHelper.GetPayloadAsync<List<ReminderDto>>(response);
        reminders.Should().NotBeNull();
        reminders!.Should().Contain(r => r.Id == _mainReminder.Id && r.DueDate > DateTime.UtcNow);
    }

    [Fact]
    public async Task ShouldGetNotViewedByUser()
    {
        // Act
        var response = await Client.GetAsync("reminders/get-not-viewed-by-user");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminders = await JsonHelper.GetPayloadAsync<List<ReminderDto>>(response);
        reminders.Should().NotBeNull();
        reminders!.Should().NotContain(r => r.IsViewed);
    }

    [Fact]
    public async Task ShouldUpdateReminder()
    {
        // Arrange
        var updatedTitle = "Updated Test Reminder";
        var updatedDueDate = DateTime.UtcNow.AddDays(2);
        var updateReminderDto = new UpdateReminderDto
        {
            Title = updatedTitle,
            DueDate = updatedDueDate,
            Type = _mainReminderType.Id,
            ContainerId = _containersData.Id
        };

        // Act
        var response = await Client.PutAsJsonAsync($"reminders/update/{_mainReminder.Id}", updateReminderDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedReminder = await JsonHelper.GetPayloadAsync<ReminderDto>(response);
        updatedReminder.Should().NotBeNull();

        var reminderFromDatabase = await Context.Reminders
            .FirstOrDefaultAsync(x => x.Id == updatedReminder!.Id);

        reminderFromDatabase.Should().NotBeNull();
        reminderFromDatabase!.Title.Should().Be(updatedTitle);
        reminderFromDatabase.TypeId.Should().Be(_mainReminderType.Id);
        reminderFromDatabase.ContainerId.Should().Be(_containersData.Id);
    }

    [Fact]
    public async Task ShouldDeleteReminder()
    {
        // Arrange
        var reminderId = _mainReminder.Id;

        // Act
        var response = await Client.DeleteAsync($"reminders/delete/{reminderId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var reminderFromDatabase = await Context.Reminders
            .FirstOrDefaultAsync(x => x.Id == reminderId);
        reminderFromDatabase.Should().BeNull();
    }

    public async Task InitializeAsync()
    {
        var userEntity = new UserEntity
        {
            Id = UserId,
            Email = "qwerty@gmail.com",
            PasswordHash = "fdsafdsafsad",
            RoleId = "Administrator"
        };

        var reminderTypeEntity = new ReminderTypeEntity
        {
            Id = _mainReminderType.Id,
            Name = _mainReminderType.Name,
            CreatedBy = UserId
        };

        var reminderEntity = new ReminderEntity
        {
            Id = _mainReminder.Id,
            ContainerId = _containersData.Id,
            Title = _mainReminder.Title,
            DueDate = _mainReminder.DueDate,
            TypeId = _mainReminderType.Id,
            CreatedBy = UserId,
            CreatedAt = DateTime.UtcNow,
            IsViewed = false
        };
        
        var containerTypeEntity = new ContainerTypeEntity
        {
            Id = _containersData.TypeId,
            Name = _containersData.Name,
            CreatedBy = UserId
        };
        
        var containerEntity = new ContainerEntity
        {
            Id = _containersData.Id,
            Name = _containersData.Name,
            CreatedBy = UserId,
            UniqueCode = "TEST123",
            TypeId = _containersData.TypeId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            ModifiedBy = UserId
        };

        await Context.Users.AddAsync(userEntity);
        await Context.ContainerTypes.AddAuditableAsync(containerTypeEntity);
        await Context.Containers.AddAuditableAsync(containerEntity);
        await Context.ReminderTypes.AddAuditableAsync(reminderTypeEntity);
        await Context.Reminders.AddAuditableAsync(reminderEntity);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Reminders.RemoveRange(Context.Reminders);
        Context.ReminderTypes.RemoveRange(Context.ReminderTypes);
        Context.Users.RemoveRange(Context.Users);
        Context.Containers.RemoveRange(Context.Containers);
        Context.ContainerTypes.RemoveRange(Context.ContainerTypes);
        await SaveChangesAsync();
    }
}