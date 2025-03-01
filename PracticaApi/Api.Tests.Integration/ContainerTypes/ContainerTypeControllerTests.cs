using System.Net;
using System.Net.Http.Json;
using Api.Dtos.ContainersType;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.ContainerTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.ContainerTypes;

public class ContainersTypeControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly ContainerType _mainContainerType;

    public ContainersTypeControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainContainerType = ContainerTypeData.MainContainerType;
    }

    [Fact]
    public async Task ShouldCreateContainerType()
    {
        // Arrange
        var containerTypeName = "New Test Container Type";
        var request = new CreateUpdateContainerTypeDto { Name = containerTypeName };

        // Act
        var response = await Client.PostAsJsonAsync("containers-type/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var containerTypeFromResponse = await response.ToResponseModel<ContainerTypeDto>();
        var containerTypeId = containerTypeFromResponse.Id!.Value;

        var containerTypeFromDatabase = await Context.ContainerTypes.FirstOrDefaultAsync(x => x.Id == containerTypeId);

        containerTypeFromDatabase.Should().NotBeNull();
        containerTypeFromDatabase!.Name.Should().Be(containerTypeName);
    }


    [Fact]
    public async Task ShouldUpdateContainerType()
    {
        var newName = "Updated Container Type";
        var request = new CreateUpdateContainerTypeDto { Name = newName };

        var response = await Client.PutAsJsonAsync($"containers-type/update/{_mainContainerType.Id}", request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var containerTypeFromDatabase = await Context.ContainerTypes.FirstOrDefaultAsync(x => x.Id == _mainContainerType.Id);
        containerTypeFromDatabase.Should().NotBeNull();
        containerTypeFromDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldNotUpdateContainerTypeBecauseNotFound()
    {
        var request = new CreateUpdateContainerTypeDto { Name = "Non-Existent Container" };

        var response = await Client.PutAsJsonAsync($"containers-type/update/{Guid.NewGuid()}", request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldDeleteContainerType()
    {
        var containerId = _mainContainerType.Id;

        var response = await Client.DeleteAsync($"containers-type/delete/{containerId}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var containerFromDatabase = await Context.ContainerTypes.FirstOrDefaultAsync(x => x.Id == containerId);
        containerFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteContainerTypeBecauseNotFound()
    {
        var response = await Client.DeleteAsync($"containers-type/delete/{Guid.NewGuid()}");

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldGetAllContainerTypes()
    {
        var response = await Client.GetAsync("containers-type/get-all");

        response.IsSuccessStatusCode.Should().BeTrue();

        var containers = await response.ToResponseModel<List<ContainerTypeDto>>();
        containers.Should().NotBeEmpty();
    }

    public async Task InitializeAsync()
    {
        var containerEntity = new ContainerTypeEntity
        {
            Id = _mainContainerType.Id,
            Name = _mainContainerType.Name,
            CreatedBy = UserId
        };

        await Context.Users.AddAsync(new UserEntity{Id = UserId,Email = "qwerty@gmail.com",PasswordHash = "fdsafdsafsad", RoleId = "Administrator"});
        await Context.ContainerTypes.AddAuditableAsync(containerEntity);
        await SaveChangesAsync();
    }



    public async Task DisposeAsync()
    {
        Context.ContainerTypes.RemoveRange(Context.ContainerTypes);
       //Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}