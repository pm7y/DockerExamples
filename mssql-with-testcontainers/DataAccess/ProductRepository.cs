using Microsoft.Data.SqlClient;

namespace DataAccess;

/// <summary>
///     Repository for managing products in SQL Server.
///     Note: Database schema is managed by DbUp migrations - see DatabaseMigrator.
/// </summary>
public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    ///     Adds a new product to the database.
    /// </summary>
    /// <param name="name">The name of the product.</param>
    /// <param name="price">The price of the product.</param>
    /// <param name="category">The category of the product (optional).</param>
    /// <returns>The ID of the newly created product.</returns>
    public async Task<int> AddProductAsync(string name, decimal price, string? category = null)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = """
                           INSERT INTO Products (Name, Price, Category)
                           OUTPUT INSERTED.Id
                           VALUES (@Name, @Price, @Category)
                           """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", name);
        command.Parameters.AddWithValue("@Price", price);
        command.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);

        return (int)(await command.ExecuteScalarAsync())!;
    }

    /// <summary>
    ///     Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to retrieve.</param>
    /// <returns>The product if found, otherwise null.</returns>
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT Id, Name, Price, Category FROM Products WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDecimal(2),
                reader.IsDBNull(3) ? null : reader.GetString(3));

        return null;
    }

    /// <summary>
    ///     Retrieves all products from the database.
    /// </summary>
    /// <returns>A list of all products.</returns>
    public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT Id, Name, Price, Category FROM Products";

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var products = new List<Product>();
        while (await reader.ReadAsync())
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDecimal(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));

        return products;
    }

    /// <summary>
    ///     Retrieves products by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>A list of products in the specified category.</returns>
    public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(string category)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "SELECT Id, Name, Price, Category FROM Products WHERE Category = @Category";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Category", category);
        await using var reader = await command.ExecuteReaderAsync();

        var products = new List<Product>();
        while (await reader.ReadAsync())
            products.Add(new Product(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetDecimal(2),
                reader.IsDBNull(3) ? null : reader.GetString(3)));

        return products;
    }

    /// <summary>
    ///     Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <returns>True if the product was deleted, false if not found.</returns>
    public async Task<bool> DeleteProductAsync(int id)
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string sql = "DELETE FROM Products WHERE Id = @Id";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}