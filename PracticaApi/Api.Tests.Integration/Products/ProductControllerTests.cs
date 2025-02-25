using System.Net.Http.Json;
using Api.Dtos.Products;
using DataAccessLayer.Entities.Products;
using Domain.ProductModels;
using FluentAssertions;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration.Products;

public class ProductsControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly Product _mainProduct = ProductsData.MainProduct;

    public ProductsControllerTests(IntegrationTestWebFactory factory) : base(factory) {}

    [Fact]
    public async Task ShouldGetAllProducts()
    {
        // Act
        var response = await Client.GetAsync("products/get-all");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
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
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
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
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
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
            TypeId = Guid.NewGuid()
        };

        // Act
        var response = await Client.PostAsJsonAsync("products/add", newProduct);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(newProduct.Name);
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
            TypeId = _mainProduct.TypeId
        };

        await Context.Products.AddAsync(productEntity);
        await SaveChangesAsync();
    }


    public async Task DisposeAsync()
    {
        Context.Products.RemoveRange(Context.Products);
        await SaveChangesAsync();
    }
}
