using System.Net;
using System.Net.Http.Json;
using Api.Dtos.Products;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.Products;
using Domain.ProductTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class ProductsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Product _mainProduct;
    private readonly ProductType _mainProductType = ProductTypeData.MainProductType;

    public ProductsControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
        _mainProduct = ProductsData.MainProduct(_mainProductType.Id);
    }

    [Fact]
    public async Task ShouldAddProduct()
    {
        // Arrange
        var newProduct = new CreateProductDto
        {
            Name = "New Test Product",
            Description = "Test description",
            ManufactureDate = DateTime.UtcNow,
            TypeId = _mainProductType.Id,
        };

        // Act
        var response = await Client.PostAsJsonAsync("products/add", newProduct);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var createdProduct = await JsonHelper.GetPayloadAsync<Product>(response);
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(newProduct.Name);
    }

    [Fact]
    public async Task ShouldGetAllProducts()
    {
        // Act
        var response = await Client.GetAsync("products/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var products = await JsonHelper.GetPayloadAsync<List<ProductDto>>(response);
        products.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldGetProductById()
    {
        // Arrange
        var productId = _mainProduct.Id;

        // Act
        var response = await Client.GetAsync($"products/get-by-id/{productId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var product = await JsonHelper.GetPayloadAsync<ProductDto>(response);
        product.Should().NotBeNull();
        product!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task ShouldReturnNotFoundForNonExistentProduct()
    {
        // Arrange
        var nonExistentProductId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"products/get-by-id/{nonExistentProductId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldUpdateProduct()
    {
        // Arrange
        var newName = "New Test Product Updated";
        var newDescription = "Test description Updated";

        var updateProductDto = new UpdateProductDto
        {
            Name = newName,
            Description = newDescription,
            ManufactureDate = DateTime.UtcNow,
            TypeId = _mainProductType.Id,
        };

        // Act
        var response = await Client.PutAsJsonAsync($"products/update/{_mainProduct.Id}", updateProductDto);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var updatedProduct = await JsonHelper.GetPayloadAsync<ProductDto>(response);

        var manufacturerFromDataBase = await Context.Products
            .FirstOrDefaultAsync(x => x.Id == updatedProduct!.Id);

        manufacturerFromDataBase.Should().NotBeNull();

        manufacturerFromDataBase!.Name.Should().Be(newName);
        manufacturerFromDataBase!.Description.Should().Be(newDescription);
    }


    [Fact]
    public async Task ShouldDeleteProduct()
    {
        // Arrange
        var productId = _mainProduct.Id;

        // Act
        var response = await Client.DeleteAsync($"products/delete/{productId}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    public async Task InitializeAsync()
    {
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
        await SaveChangesAsync();
    }


    public async Task DisposeAsync()
    {
        Context.Products.RemoveRange(Context.Products);
        Context.ProductTypes.RemoveRange(Context.ProductTypes);
        Context.Users.RemoveRange(Context.Users);
        await SaveChangesAsync();
    }
}