using Testcontainers.MsSql;
using Xunit;

namespace DataAccess.Tests;

/// <summary>
///     Integration tests with complete isolation - each test gets its own container.
///     Schema is set up via DbUp migrations when the container starts.
///     This approach is slower (15-30s per test) but guarantees zero test pollution.
///     Use sparingly when absolute isolation is required.
/// </summary>
public class IsolatedProductRepositoryTests : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-ubuntu-22.04")
        .Build();

    private ProductRepository _repository = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Run DbUp migrations to set up the schema
        var result = DatabaseMigrator.MigrateSilently(_container.GetConnectionString());
        if (!result.Successful) throw new Exception($"Database migration failed: {result.Error}");

        _repository = new ProductRepository(_container.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task FreshDatabase_ContainsOnlySeededProducts()
    {
        // This test has a completely isolated database
        var products = await _repository.GetAllProductsAsync();

        // Should have exactly 3 seeded products from migration 002
        Assert.Equal(3, products.Count);
        Assert.Contains(products, p => p.Name == "Widget" && p.Price == 9.99m);
        Assert.Contains(products, p => p.Name == "Gadget" && p.Price == 19.99m);
        Assert.Contains(products, p => p.Name == "Gizmo" && p.Price == 29.99m);
    }

    [Fact]
    public async Task SeededProducts_HaveGeneralCategory()
    {
        // Migration 003 adds Category column and sets existing products to "General"
        var products = await _repository.GetProductsByCategoryAsync("General");

        Assert.Equal(3, products.Count);
    }

    [Fact]
    public async Task AddAndRetrieveProduct_InIsolation()
    {
        // Add a product
        var id = await _repository.AddProductAsync("Isolated Widget", 29.99m, "Special");

        // Retrieve it
        var product = await _repository.GetProductByIdAsync(id);

        // Verify
        Assert.NotNull(product);
        Assert.Equal("Isolated Widget", product.Name);
        Assert.Equal(29.99m, product.Price);
        Assert.Equal("Special", product.Category);

        // Should have seeded products (3) plus our new one (1)
        var allProducts = await _repository.GetAllProductsAsync();
        Assert.Equal(4, allProducts.Count);
    }
}