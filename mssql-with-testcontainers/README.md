# SQL Server Integration Testing with Testcontainers

This is a complete working example demonstrating different approaches to integration testing with SQL Server using [Testcontainers for .NET](https://dotnet.testcontainers.org/) and [DbUp](https://dbup.readthedocs.io/) for database migrations.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/)

## Quick Start

```bash
# Restore packages
dotnet restore

# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal
```

## Project Structure

```
TestcontainersDemo/
├── DataAccess/                          # Class library with data access code
│   ├── Migrations/                      # DbUp SQL migration scripts
│   │   ├── 001_CreateProductsTable.sql  # Creates Products table
│   │   ├── 002_SeedInitialProducts.sql  # Seeds initial data
│   │   └── 003_AddProductCategory.sql   # Adds Category column
│   ├── DatabaseMigrator.cs              # DbUp migration runner
│   ├── Product.cs                       # Product record type
│   └── ProductRepository.cs             # Repository with CRUD operations
│
├── DataAccess.Tests/                    # xUnit test project
│   ├── Fixtures/
│   │   ├── SqlServerFixture.cs          # Basic fixture (IClassFixture)
│   │   └── DatabaseCollection.cs        # Collection fixture (ICollectionFixture)
│   │
│   ├── BasicProductRepositoryTests.cs   # Tests using IClassFixture
│   ├── IsolatedProductRepositoryTests.cs # Tests with per-test containers
│   ├── RespawnProductRepositoryTests.cs # Tests using Respawn for fast resets
│   └── CollectionProductRepositoryTests.cs # Tests sharing container via collection
│
└── TestcontainersDemo.sln
```

## Database Migrations with DbUp

This example uses [DbUp](https://dbup.readthedocs.io/) to manage database schema. Migrations are:
- Embedded as resources in the `DataAccess` assembly
- Run automatically when test fixtures initialize
- Tracked in a `SchemaVersions` table to prevent re-running

### Migration Scripts

| Script | Purpose |
|--------|---------|
| `001_CreateProductsTable.sql` | Creates the Products table with Id, Name, Price, CreatedAt |
| `002_SeedInitialProducts.sql` | Seeds 3 initial products (Widget, Gadget, Gizmo) |
| `003_AddProductCategory.sql` | Adds Category column, updates existing rows to "General" |

### Running Migrations

```csharp
// Run migrations with console logging
var result = DatabaseMigrator.Migrate(connectionString);

// Run migrations silently (for tests)
var result = DatabaseMigrator.MigrateSilently(connectionString);

// Check if migrations are needed
bool upToDate = DatabaseMigrator.IsUpToDate(connectionString);
```

## Test Approaches Demonstrated

### 1. Basic Tests with `IClassFixture`
**File:** `BasicProductRepositoryTests.cs`

- Container shared across all tests in the class
- Migrations run once when fixture starts
- Fast (one container startup per class)
- Tests may affect each other (shared database state)
- Database starts with seeded data from migration 002

### 2. Isolated Tests with Per-Test Containers
**File:** `IsolatedProductRepositoryTests.cs`

- Each test gets its own container
- Migrations run for each test
- Slow (15-30s startup per test)
- Complete isolation guaranteed
- Database always starts with exactly 3 seeded products

### 3. Collection Tests with `ICollectionFixture`
**Files:** `DatabaseCollection.cs`, `CollectionProductRepositoryTests.cs`

- Container shared across multiple test classes
- Migrations run once for entire collection
- Very efficient (one container for entire collection)
- Tests may affect each other

### 4. Respawn Tests (Recommended)
**File:** `RespawnProductRepositoryTests.cs`

- Container shared via `ICollectionFixture`
- Database reset before each test (~50ms)
- Complete isolation with fast performance
- Schema preserved, data cleared (except SchemaVersions table)
- Best for most real-world scenarios

## Performance Comparison

| Approach | Container Starts | Migrations Run | Typical Overhead |
|----------|------------------|----------------|------------------|
| Per-test | Every test | Every test | 15-30s **per test** |
| Per-class (`IClassFixture`) | Once per class | Once per class | 15-30s per class |
| Per-collection (`ICollectionFixture`) | Once total | Once total | 15-30s total |
| Collection + Respawn | Once total | Once total | 15-30s startup + ~50ms per reset |

## Key Packages

- **[Testcontainers.MsSql](https://www.nuget.org/packages/Testcontainers.MsSql)** - SQL Server container support
- **[dbup-sqlserver](https://www.nuget.org/packages/dbup-sqlserver)** - Database migrations
- **[Respawn](https://www.nuget.org/packages/Respawn)** - Fast database reset between tests
- **[xunit](https://www.nuget.org/packages/xunit)** - Test framework

## Using Respawn with DbUp

When using Respawn with DbUp, configure Respawn to ignore the `SchemaVersions` table:

```csharp
_respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions
{
    DbAdapter = DbAdapter.SqlServer,
    SchemasToInclude = ["dbo"],
    TablesToIgnore = ["SchemaVersions"]  // DbUp's migration tracking table
});
```

This ensures migrations aren't re-run after each reset.

## Related Blog Post

This example accompanies the blog post: [Part III: SQL Server Integration Testing with Testcontainers](https://pmcilreavy.github.io/post/2026-01-07-sql-server-integration-testing-with-testcontainers/)

## Related Examples

- [Part I: SQL Server with Docker Compose](../2024-04-05-sql-server-with-docker-compose/)
- [Part II: Initialising SQL Server with Docker Compose](../2024-04-06-sql-server-init-docker-compose/)

## License

MIT
