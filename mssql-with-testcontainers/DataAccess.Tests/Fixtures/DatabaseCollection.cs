using Testcontainers.MsSql;
using Xunit;

namespace DataAccess.Tests.Fixtures;

/// <summary>
///     Defines a test collection that shares a single SQL Server container
///     across multiple test classes. This is more efficient than IClassFixture
///     when you have many test classes that need database access.
/// </summary>
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<SharedSqlServerFixture>
{
    // This class has no code - it's just a marker that links
    // the collection name "Database" to the SharedSqlServerFixture
}

/// <summary>
///     Fixture for the Database collection. This container is shared across
///     all test classes that use [Collection("Database")].
///     Automatically runs DbUp migrations when the container starts.
/// </summary>
public class SharedSqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU12-ubuntu-22.04")
        .Build();

    /// <summary>
    ///     Gets the connection string to the running SQL Server container.
    /// </summary>
    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Run DbUp migrations to set up the schema
        var result = DatabaseMigrator.MigrateSilently(ConnectionString);
        if (!result.Successful) throw new Exception($"Database migration failed: {result.Error}");
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}