namespace SmartServices.Framework.Utilities.State;

/// <summary>
/// Provides distributed caching using Dapr state block.
/// Enables state management and caching across microservices.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="ttlSeconds">The time-to-live in seconds (optional).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        int? ttlSeconds = null,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The cached value, or null if not found.</returns>
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Deletes a value from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default);
}
