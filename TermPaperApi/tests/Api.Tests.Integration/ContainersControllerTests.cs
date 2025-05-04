using System.Net;
using System.Net.Http.Json;
using Api.Dtos.Containers;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.Containers;
using Domain.ContainerTypes;
using Domain.Products;
using Domain.ProductTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class ContainersControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Container? _mainContainer;
    private readonly ContainerType _mainContainerType = ContainerTypeData.MainContainerType;
    private readonly Product _mainProduct;
    private readonly ProductType _mainProductType = ProductTypeData.MainProductType;

    public ContainersControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainContainer = ContainersData.MainContainer(_mainContainerType.Id);
        _mainProduct = ProductsData.MainProduct(_mainProductType.Id);
    }

    [Fact]
    public async Task ShouldGetAllContainers()
    {
        // Act
        var response = await Client.GetAsync("containers/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var containers = await JsonHelper.GetPayloadAsync<List<ContainerDto>>(response);
        containers.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetContainerById()
    {
        // Arrange
        var containerId = _mainContainer.Id;

        // Act
        var response = await Client.GetAsync($"containers/get-by-id/{containerId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var container = await JsonHelper.GetPayloadAsync<ContainerDto>(response);
        container.Should().NotBeNull();
        container!.Id.Should().Be(containerId);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNonExistentContainer()
    {
        // Arrange
        var nonExistentContainerId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"containers/get-by-id/{nonExistentContainerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldAddContainer()
    {
        // Arrange
        var newContainer = new CreateContainerDto
        {
            Name = "New Test Container",
            Volume = 100,
            Notes = "Test notes",
            TypeId = _mainContainerType.Id,
        };

        // Act
        var response = await Client.PostAsJsonAsync("containers/add", newContainer);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var createdContainer = await JsonHelper.GetPayloadAsync<Container>(response);
        createdContainer.Should().NotBeNull();
        createdContainer!.Name.Should().Be(newContainer.Name);
    }

    [Fact]
    public async Task ShouldUpdateContainer()
    {
        // Arrange
        var newName = "New Test Container Updated";
        var newNotes = "Test notes Updated";

        var updateContainerDto = new UpdateContainerDto
        {
            Name = newName,
            Volume = 200,
            Notes = newNotes,
        };

        // Act
        var response = await Client.PutAsJsonAsync($"containers/update/{_mainContainer.Id}", updateContainerDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedContainer = await JsonHelper.GetPayloadAsync<Container>(response);

        var containerFromDatabase = await Context.Containers
            .FirstOrDefaultAsync(x => x.Id == updatedContainer!.Id);

        containerFromDatabase.Should().NotBeNull();
        containerFromDatabase!.Name.Should().Be(newName);
        containerFromDatabase!.Notes.Should().Be(newNotes);
    }

    [Fact]
    public async Task ShouldDeleteContainer()
    {
        // Arrange
        var containerId = _mainContainer.Id;

        // Act
        var response = await Client.DeleteAsync($"containers/delete/{containerId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldSetContainerContent()
    {
        // Arrange
        var productId = _mainProduct.Id;
        var setContentDto = new SetContainerContentDto
        {
            ProductId = productId
        };

        // Act
        var response = await Client.PutAsJsonAsync($"containers/set-content/{_mainContainer.Id}", setContentDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedContainer = await JsonHelper.GetPayloadAsync<Container>(response);

        var containerFromDatabase = await Context.Containers
            .FirstOrDefaultAsync(x => x.Id == updatedContainer!.Id);

        containerFromDatabase.Should().NotBeNull();
        containerFromDatabase!.ProductId.Should().Be(productId);
    }

    [Fact]
    public async Task ShouldClearContainerContent()
    {
        // Arrange
        var productId = _mainProduct.Id;
        var setContentDto = new SetContainerContentDto
        {
            ProductId = productId
        };

        await Client.PutAsJsonAsync($"containers/set-content/{_mainContainer.Id}", setContentDto);

        var containerId = _mainContainer.Id;

        // Act
        var response = await Client.PutAsync($"containers/clear-content/{containerId}", null);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedContainer = await JsonHelper.GetPayloadAsync<Container>(response);

        var containerFromDatabase = await Context.Containers
            .FirstOrDefaultAsync(x => x.Id == updatedContainer!.Id);

        containerFromDatabase.Should().NotBeNull();
        containerFromDatabase!.ProductId.Should().BeNull();
    }

    [Fact]
    public async Task ShouldGetContainersByFillStatus()
    {
        // Arrange
        var isEmpty = true;

        // Act
        var response = await Client.GetAsync($"containers/get-by-fill-status/{isEmpty}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var containers = await JsonHelper.GetPayloadAsync<List<ContainerDto>>(response);
        containers.Should().NotBeNull();
        containers!.All(c => c.IsEmpty == isEmpty).Should().BeTrue();
    }

    [Fact]
    public async Task ShouldGetContainersByProductType()
    {
        // Arrange
        var productTypeId = _mainContainerType.Id;

        // Act
        var response = await Client.GetAsync($"containers/get-by-product-type/{productTypeId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var containers = await JsonHelper.GetPayloadAsync<List<ContainerDto>>(response);
        containers.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetContainersByProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"containers/get-by-product/{productId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var containers = await JsonHelper.GetPayloadAsync<List<ContainerDto>>(response);
        containers.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetEmptyContainersByLastProduct()
    {
        // Arrange
        var lastProductId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"containers/get-empty-by-last-product/{lastProductId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var containers = await JsonHelper.GetPayloadAsync<List<ContainerDto>>(response);
        containers.Should().NotBeNull();
    }

    public async Task InitializeAsync()
    {
        var containerEntity = new ContainerEntity
        {
            Id = _mainContainer.Id,
            Name = _mainContainer.Name,
            Volume = _mainContainer.Volume,
            Notes = _mainContainer.Notes,
            TypeId = _mainContainer.TypeId,
            UniqueCode = "TEST123",
            CreatedBy = UserId
        };

        var containerTypeEntity = new ContainerTypeEntity
        {
            Id = _mainContainerType.Id,
            Name = _mainContainerType.Name,
            CreatedBy = UserId
        };

        var productEntity = new ProductEntity
        {
            Id = _mainProduct.Id,
            Name = _mainProduct.Name,
            Description = _mainProduct.Description,
            ManufactureDate = _mainProduct.ManufactureDate,
            TypeId = _mainProduct.TypeId,
            CreatedBy = UserId
        };

        var productTypeEntity = new ProductTypeEntity
        {
            Id = _mainProductType.Id,
            Name = _mainProductType.Name,
            CreatedBy = UserId
        };

        await Context.Users.AddAsync(new UserEntity
            { Id = UserId, Email = "qwerty@gmail.com", PasswordHash = "fdsafdsafsad", RoleId = "Administrator" });
        await Context.ProductTypes.AddAuditableAsync(productTypeEntity);
        await Context.Products.AddAuditableAsync(productEntity);
        await Context.ContainerTypes.AddAuditableAsync(containerTypeEntity);
        await Context.Containers.AddAuditableAsync(containerEntity);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.Products.RemoveRange(Context.Products);
        Context.ProductTypes.RemoveRange(Context.ProductTypes);
        Context.Containers.RemoveRange(Context.Containers);
        Context.ContainerTypes.RemoveRange(Context.ContainerTypes);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}