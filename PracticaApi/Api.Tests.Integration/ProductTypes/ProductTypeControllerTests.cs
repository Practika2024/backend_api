using System.Net;
using System.Net.Http.Json;
using Api.Dtos.ProductsType;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using Domain.ProductTypeModels;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.ProductTypes;

public class ProductTypeControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly ProductType _mainProductType;

    public ProductTypeControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainProductType = new ProductType { Id = Guid.NewGuid(), Name = "Main Test Product Type" };
    }

    [Fact]
    public async Task ShouldCreateProductType()
    {
        var request = new CreateUpdateProductTypeDto { Name = "New Test Product Type" };

        var response = await Client.PostAsJsonAsync("product-type/add", request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var productTypeFromResponse = await response.ToResponseModel<ProductTypeDto>();
        var productTypeId = productTypeFromResponse.Id!.Value;

        var productTypeFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == productTypeId);
        productTypeFromDatabase.Should().NotBeNull();
        productTypeFromDatabase!.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task ShouldUpdateProductType()
    {
        var newName = "Updated Product Type";
        var request = new CreateUpdateProductTypeDto { Name = newName };

        var response = await Client.PutAsJsonAsync($"product-type/update/{_mainProductType.Id}", request);

        response.IsSuccessStatusCode.Should().BeTrue();

        var productTypeFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == _mainProductType.Id);
        productTypeFromDatabase.Should().NotBeNull();
        productTypeFromDatabase!.Name.Should().Be(newName);
    }

    [Fact]
    public async Task ShouldNotUpdateProductTypeBecauseNotFound()
    {
        var request = new CreateUpdateProductTypeDto { Name = "Non-Existent Product" };

        var response = await Client.PutAsJsonAsync($"product-type/update/{Guid.NewGuid()}", request);

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDeleteProductType()
    {
        var productId = _mainProductType.Id;

        var response = await Client.DeleteAsync($"product-type/delete/{productId}");

        response.IsSuccessStatusCode.Should().BeTrue();

        var productFromDatabase = await Context.ProductTypes.FirstOrDefaultAsync(x => x.Id == productId);
        productFromDatabase.Should().BeNull();
    }

    [Fact]
    public async Task ShouldNotDeleteProductTypeBecauseNotFound()
    {
        var response = await Client.DeleteAsync($"product-type/delete/{Guid.NewGuid()}");

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldGetAllProductTypes()
    {
        var response = await Client.GetAsync("product-type/get-all");

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
            CreatedBy = Guid.Parse("4f4ef8f5-786a-4aeb-bae8-11c4cc5d20b9") // Correct conversion of string to Guid
        };

        await Context.ProductTypes.AddAsync(productEntity);
        await Context.Users.AddAsync(new UserEntity{Id = UserId,Email = "qwerty@gmail.com",PasswordHash = "fdsafdsafsad", RoleId = "Administrator"});
        await SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Context.ProductTypes.RemoveRange(Context.ProductTypes);
        await SaveChangesAsync();
    }
}
