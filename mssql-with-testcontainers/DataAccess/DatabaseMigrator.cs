using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace DataAccess;

/// <summary>
///     Handles database migrations using DbUp.
///     Migrations are embedded SQL scripts that run in alphabetical order.
/// </summary>
public static class DatabaseMigrator
{
    /// <summary>
    ///     Runs all pending migrations against the specified database.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The result of the migration operation.</returns>
    public static DatabaseUpgradeResult Migrate(string connectionString)
    {
        // Ensure the database exists (creates it if it doesn't)
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToConsole()
            .Build();

        return upgrader.PerformUpgrade();
    }

    /// <summary>
    ///     Runs all pending migrations silently (no console output).
    ///     Useful for test scenarios.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The result of the migration operation.</returns>
    public static DatabaseUpgradeResult MigrateSilently(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .WithTransaction()
            .LogToNowhere()
            .Build();

        return upgrader.PerformUpgrade();
    }

    /// <summary>
    ///     Gets a list of scripts that have not yet been executed.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>List of pending script names.</returns>
    public static IEnumerable<string> GetPendingMigrations(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToNowhere()
            .Build();

        return upgrader.GetScriptsToExecute().Select(s => s.Name);
    }

    /// <summary>
    ///     Checks if the database is up to date (no pending migrations).
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>True if no migrations are pending.</returns>
    public static bool IsUpToDate(string connectionString)
    {
        return !GetPendingMigrations(connectionString).Any();
    }
}