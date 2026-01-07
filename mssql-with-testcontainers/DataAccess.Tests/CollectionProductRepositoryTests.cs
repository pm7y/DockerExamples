using DataAccess.Tests.Fixtures;
using Xunit;

namespace DataAccess.Tests;

/// <summary>
///     Another test class in the "Database" collection, demonstrating that
///     multiple test classes can share the same SQL Server container.
///     Schema is set up via DbUp migrations in the fixture.
///     Note: Without Respawn, tests in different classes can affect each other
///     because they share the same database. Use with caution or add Respawn.
/// </summary>
[Collection("Database")]
public class CollectionProductRepositoryTests
{
    private readonly SharedSqlServerFixture _fixture;

    public CollectionProductRepositoryTests(SharedSqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CanAddProductInSharedContainer()
    {
        // Arrange
        var repository = new ProductRepository(_fixture.ConnectionString);

        // Act
        var id = await repository.AddProductAsync("Shared Container Product", 15.00m, "Shared");

        // Assert
        Assert.True(id > 0);

        var product = await repository.GetProductByIdAsync(id);
        Assert.NotNull(product);
        Assert.Equal("Shared Container Product", product.Name);
        Assert.Equal("Shared", product.Category);
    }

    [Fact]
    public async Task ContainerIsSharedAcrossTestClasses()
    {
        // This test runs in the same container as RespawnProductRepositoryTests
        // The container is started once and shared across both test classes

        var repository = new ProductRepository(_fixture.ConnectionString);

        // We can interact with the same database
        var id = await repository.AddProductAsync("Cross-Class Product", 25.00m);

        Assert.True(id > 0);
    }

    [Fact]
    public async Task MigrationsHaveAlreadyRun()
    {
        // Verify that migrations were executed by the fixture
        Assert.True(DatabaseMigrator.IsUpToDate(_fixture.ConnectionString));
    }
}