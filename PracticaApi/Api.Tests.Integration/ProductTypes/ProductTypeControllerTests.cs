using System.Net;
using System.Net.Http.Json;
using Api.Dtos.ProductsType;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.ProductTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.ProductTypes;

public class ProductTypeControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly ProductType _mainProductType;

    public ProductTypeControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainProductType = ProductTypeData.MainProductType;
    }

    [Fact]
    public async Task ShouldCreateProductType()
    {
        // Arrange
        var productTypeName = "New Test Product Type";
        var request = new CreateUpdateProductTypeDto { Name = productTypeName };

        // Act
        var response = await Client.PostAsJsonAsync("products-type/add", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var productTypeFromResponse = await response.ToResponseModel<ProductTypeDto>();
        var productTypeId = productTypeFromResponse.Id!.Value;

        var productTypeFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == productTypeId);

        productTypeFromDatabase.Should().NotBeNull();
        productTypeFromDatabase!.Name.Should().Be(productTypeName);
    }

    [Fact]
    public async Task ShouldUpdateProductType()
    {
        var newName = "Updated Product Type";
        var request = new CreateUpdateProductTypeDto { Name = newName };

        var response = await Client.PutAsJsonAsync($"products-type/update/{_mainProductType.Id}", request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var productTypeFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == _mainProductType.Id);
        productTypeFromDatabase.Should().NotBeNull();
        productTypeFromDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldNotUpdateProductTypeBecauseNotFound()
    {
        var request = new CreateUpdateProductTypeDto { Name = "Non-Existent Product" };

        var response = await Client.PutAsJsonAsync($"products-type/update/{Guid.NewGuid()}", request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldDeleteProductType()
    {
        var productId = _mainProductType.Id;

        var response = await Client.DeleteAsync($"products-type/delete/{productId}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var productFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == productId);
        productFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteProductTypeBecauseNotFound()
    {
        var response = await Client.DeleteAsync($"products-type/delete/{Guid.NewGuid()}");

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldGetAllProductTypes()
    {
        var response = await Client.GetAsync("products-type/get-all");

        response.IsSuccessStatusCode.Should().BeTrue();

        var products = await response.ToResponseModel<List<ProductTypeDto>>();
        products.Should().NotBeEmpty();
    }

    public async Task InitializeAsync()
    {
        var productEntity = new ProductTypeEntity
        {
            Id = _mainProductType.Id,
            Name = _mainProductType.Name,
            CreatedBy = UserId
        };

        await Context.Users.AddAsync(new UserEntity { Id = UserId, Email = "qwerty@gmail.com", PasswordHash = "fdsafdsafsad", RoleId = "Administrator" });
        await Context.ProductTypes.AddAuditableAsync(productEntity);
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ProductTypes.RemoveRange(Context.ProductTypes);
       // Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}
