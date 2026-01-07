using Testcontainers.MsSql;
using Xunit;

namespace DataAccess.Tests.Fixtures;

/// <summary>
///     Fixture that manages the lifecycle of a SQL Server container.
///     Use with IClassFixture to share a container across all tests in a class.
///     Automatically runs DbUp migrations when the container starts.
/// </summary>
public class SqlServerFixture : IAsyncLifetime
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