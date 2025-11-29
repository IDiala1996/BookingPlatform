using Dapr.Client;
using System.Text.Json;

namespace SmartServices.Framework.Utilities.State;

/// <summary>
/// Implementation of ICacheService using Dapr state block.
/// </summary>
public class DaprCacheService : ICacheService
{
    private readonly DaprClient _daprClient;
    private readonly string _storeName;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the DaprCacheService class.
    /// </summary>
    /// <param name="daprClient">The Dapr client.</param>
    /// <param name="storeName">The name of the Dapr state store (default: "statestore").</param>
    public DaprCacheService(DaprClient daprClient, string storeName = "statestore")
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        _storeName = storeName ?? throw new ArgumentNullException(nameof(storeName));
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(
        string key,
        T value,
        int? ttlSeconds = null,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        try
        {
            var options = new List<StateOptions>();

            // Add TTL if specified
            if (ttlSeconds.HasValue && ttlSeconds > 0)
            {
                // Note: TTL support depends on state store implementation
                // This is placeholder for future TTL support
            }

            await _daprClient.SaveStateAsync(_storeName, key, value, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new CacheException($"Failed to set cache key '{key}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
        where T : class
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        try
        {
            var value = await _daprClient.GetStateAsync<T>(_storeName, key, cancellationToken: cancellationToken);
            return value;
        }
        catch (Dapr.DaprException ex) when (ex.InnerException?.Message.Contains("state not found") == true)
        {
            return null;
        }
        catch (Exception ex)
        {
            throw new CacheException($"Failed to get cache key '{key}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        try
        {
            await _daprClient.DeleteStateAsync(_storeName, key, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            throw new CacheException($"Failed to delete cache key '{key}'.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));

        try
        {
            var value = await _daprClient.GetStateAsync<object>(_storeName, key, cancellationToken: cancellationToken);
            return value != null;
        }
        catch (Dapr.DaprException ex) when (ex.InnerException?.Message.Contains("state not found") == true)
        {
            return false;
        }
        catch (Exception ex)
        {
            throw new CacheException($"Failed to check cache key '{key}'.", ex);
        }
    }
}

/// <summary>
/// Exception thrown when cache operations fail.
/// </summary>
public class CacheException : Exception
{
    /// <summary>
    /// Initializes a new instance of the CacheException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CacheException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
