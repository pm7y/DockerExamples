namespace DataAccess;

/// <summary>
///     Represents a product in the catalog.
/// </summary>
/// <param name="Id">The unique identifier for the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Price">The price of the product.</param>
/// <param name="Category">The category of the product (optional).</param>
public record Product(int Id, string Name, decimal Price, string? Category = null);