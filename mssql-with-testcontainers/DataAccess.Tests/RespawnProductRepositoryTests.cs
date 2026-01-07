using DataAccess.Tests.Fixtures;
using Respawn;
using Xunit;

namespace DataAccess.Tests;

/// <summary>
///     Integration tests using ICollectionFixture + Respawn for fast, isolated tests.
///     Schema is set up via DbUp migrations in the fixture.
///     This is the recommended approach for most projects:
///     - Container starts once (~15-30s) and is shared across all tests in the collection
///     - Database is reset before each test (~50ms) using Respawn
///     - Tests are completely isolated and can make exact assertions
///     Note: Respawn deletes all data but preserves the schema. The SchemaJournal table
///     (used by DbUp to track migrations) is excluded from resets.
/// </summary>
[Collection("Database")]
public class RespawnProductRepositoryTests : IAsyncLifetime
{
    private readonly SharedSqlServerFixture _fixture;
    private ProductRepository _repository = null!;
    private Respawner _respawner = null!;

    public RespawnProductRepositoryTests(SharedSqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        _repository = new ProductRepository(_fixture.ConnectionString);

        // Create the respawner - this analyses the database schema
        // Exclude the SchemaVersions table (DbUp's migration tracking table)
        _respawner = await Respawner.CreateAsync(_fixture.ConnectionString, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = ["dbo"],
            TablesToIgnore = ["SchemaVersions"], // DbUp's migration tracking table
        });

        // Reset the database before each test (removes all data except migration history)
        await _respawner.ResetAsync(_fixture.ConnectionString);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task EmptyDatabase_AfterRespawn()
    {
        // Database was reset by Respawn, so it should be empty
        // (Respawn removes the seeded data from migration 002)
        var products = await _repository.GetAllProductsAsync();

        Assert.Empty(products);
    }

    [Fact]
    public async Task AddProduct_ShouldReturnNewId()
    {
        var id = await _repository.AddProductAsync("Widget", 9.99m);

        Assert.True(id > 0);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsExactCount()
    {
        // Add exactly 2 products
        await _repository.AddProductAsync("Product A", 10.00m, "Category1");
        await _repository.AddProductAsync("Product B", 20.00m, "Category2");

        var products = await _repository.GetAllProductsAsync();

        // Exact count because database was reset before this test!
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetProductsByCategory_ReturnsOnlyMatchingProducts()
    {
        // Add products in different categories
        await _repository.AddProductAsync("Electronics Item", 99.99m, "Electronics");
        await _repository.AddProductAsync("Clothing Item", 49.99m, "Clothing");
        await _repository.AddProductAsync("Another Electronics", 149.99m, "Electronics");

        var electronics = await _repository.GetProductsByCategoryAsync("Electronics");

        Assert.Equal(2, electronics.Count);
        Assert.All(electronics, p => Assert.Equal("Electronics", p.Category));
    }

    [Fact]
    public async Task AddAndDelete_LeavesEmptyDatabase()
    {
        // Add a product
        var id = await _repository.AddProductAsync("Temporary", 5.00m);

        // Delete it
        await _repository.DeleteProductAsync(id);

        // Should be empty
        var products = await _repository.GetAllProductsAsync();
        Assert.Empty(products);
    }

    [Fact]
    public async Task MultipleProducts_CanBeRetrievedById()
    {
        // Add multiple products
        var id1 = await _repository.AddProductAsync("First", 10.00m);
        var id2 = await _repository.AddProductAsync("Second", 20.00m);
        var id3 = await _repository.AddProductAsync("Third", 30.00m);

        // Retrieve each one
        var product1 = await _repository.GetProductByIdAsync(id1);
        var product2 = await _repository.GetProductByIdAsync(id2);
        var product3 = await _repository.GetProductByIdAsync(id3);

        // Verify
        Assert.NotNull(product1);
        Assert.NotNull(product2);
        Assert.NotNull(product3);
        Assert.Equal("First", product1.Name);
        Assert.Equal("Second", product2.Name);
        Assert.Equal("Third", product3.Name);
    }
}