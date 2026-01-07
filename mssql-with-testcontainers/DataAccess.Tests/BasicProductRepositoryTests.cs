using DataAccess.Tests.Fixtures;
using Xunit;

namespace DataAccess.Tests;

/// <summary>
///     Basic integration tests using IClassFixture.
///     The SQL Server container is shared across all tests in this class.
///     Schema is set up via DbUp migrations in the fixture.
///     Note: Because tests share the same database, they can affect each other.
///     The database starts with 3 seeded products from migration 002.
/// </summary>
public class BasicProductRepositoryTests : IClassFixture<SqlServerFixture>
{
    private readonly SqlServerFixture _fixture;

    public BasicProductRepositoryTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddProduct_ShouldReturnNewId()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var id = await repository.AddProductAsync("New Widget", 9.99m);

        // Assert
        Assert.True(id > 0);
    }

    [Fact]
    public async Task AddProduct_WithCategory_ShouldReturnNewId()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var id = await repository.AddProductAsync("Categorized Widget", 14.99m, "Electronics");

        // Assert
        Assert.True(id > 0);
        var product = await repository.GetProductByIdAsync(id);
        Assert.NotNull(product);
        Assert.Equal("Electronics", product.Category);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);
        var id = await repository.AddProductAsync("Test Gadget", 19.99m);

        // Act
        var product = await repository.GetProductByIdAsync(id);

        // Assert
        Assert.NotNull(product);
        Assert.Equal("Test Gadget", product.Name);
        Assert.Equal(19.99m, product.Price);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var product = await repository.GetProductByIdAsync(99999);

        // Assert
        Assert.Null(product);
    }

    [Fact]
    public async Task GetAllProducts_ShouldIncludeSeededProducts()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var products = await repository.GetAllProductsAsync();

        // Assert
        // Database has 3 seeded products from migration 002, plus any added by other tests
        Assert.True(products.Count >= 3);

        // Verify seeded products exist
        Assert.Contains(products, p => p.Name == "Widget");
        Assert.Contains(products, p => p.Name == "Gadget");
        Assert.Contains(products, p => p.Name == "Gizmo");
    }

    [Fact]
    public async Task GetProductsByCategory_ShouldReturnSeededProducts()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act - seeded products have "General" category from migration 003
        var products = await repository.GetProductsByCategoryAsync("General");

        // Assert
        Assert.True(products.Count >= 3);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);
        var id = await repository.AddProductAsync("ToDelete", 5.00m);

        // Act
        var deleted = await repository.DeleteProductAsync(id);

        // Assert
        Assert.True(deleted);
        Assert.Null(await repository.GetProductByIdAsync(id));
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var deleted = await repository.DeleteProductAsync(99999);

        // Assert
        Assert.False(deleted);
    }
}