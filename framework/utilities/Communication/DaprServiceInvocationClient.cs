using Dapr.Client;

namespace SmartServices.Framework.Utilities.Communication;


/// <summary>
/// Implementation of IServiceInvocationClient using Dapr.
/// </summary>
public class DaprServiceInvocationClient : IServiceInvocationClient
{
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the DaprServiceInvocationClient class.
    /// </summary>
    /// <param name="daprClient">The Dapr client.</param>
    public DaprServiceInvocationClient(DaprClient daprClient)
    {
        _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    /// <inheritdoc/>
    public string DefaultPubSubName => DEFAULTPUBSUBCOMPONENTNAME;
    public const string DEFAULTPUBSUBCOMPONENTNAME = "pubsub";
    /// <inheritdoc />
    public async Task<TResponse> InvokeAsync<TResponse>(
        string serviceName,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));

        try
        {
            var response = await _daprClient.InvokeMethodAsync<TResponse>(
                serviceName,
                methodName,
                cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to invoke {serviceName}/{methodName}.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<TResponse> InvokeAsync<TRequest, TResponse>(
        string serviceName,
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));

        try
        {
            var response = await _daprClient.InvokeMethodAsync<TRequest, TResponse>(
                serviceName,
                methodName,
                request,
                cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to invoke {serviceName}/{methodName}.", ex);
        }
    }
    /// <inheritdoc />
    public async Task<TResponse> InvokeAsync<TResponse>(
        HttpMethod httpMethod,
        string serviceName,
        string methodName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));

        try
        {
            var response = await _daprClient.InvokeMethodAsync<TResponse>(
                httpMethod,
                serviceName,
                methodName,
                cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to invoke {serviceName}/{methodName}.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<TResponse> InvokeAsync<TRequest, TResponse>(
        HttpMethod httpMethod,
        string serviceName,
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(serviceName))
            throw new ArgumentException("Service name cannot be null or empty.", nameof(serviceName));
        if (string.IsNullOrEmpty(methodName))
            throw new ArgumentException("Method name cannot be null or empty.", nameof(methodName));

        try
        {
            var response = await _daprClient.InvokeMethodAsync<TRequest, TResponse>(
                httpMethod,
                serviceName,
                methodName,
                request,
                cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to invoke {serviceName}/{methodName}.", ex);
        }
    }

    /// <inheritdoc />
    public async Task PublishEventAsync<TEvent>(
        string topicName,
        TEvent eventData,
        CancellationToken cancellationToken = default)
        where TEvent : class
    {
        if (string.IsNullOrEmpty(DefaultPubSubName))
            throw new ArgumentException("Pub/sub name cannot be null or empty.", nameof(DefaultPubSubName));
        if (string.IsNullOrEmpty(topicName))
            throw new ArgumentException("Topic name cannot be null or empty.", nameof(topicName));

        try
        {
            await _daprClient.PublishEventAsync(DefaultPubSubName, topicName, eventData, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to publish event to {DefaultPubSubName}/{topicName}.", ex);
        }
    }

    /// <inheritdoc />
    public async Task PublishEventAsync<TEvent>(
        string pubsubName,
        string topicName,
        TEvent eventData,
        CancellationToken cancellationToken = default)
        where TEvent : class
    {
        if (string.IsNullOrEmpty(pubsubName))
            throw new ArgumentException("Pub/sub name cannot be null or empty.", nameof(pubsubName));
        if (string.IsNullOrEmpty(topicName))
            throw new ArgumentException("Topic name cannot be null or empty.", nameof(topicName));

        try
        {
            await _daprClient.PublishEventAsync(pubsubName, topicName, eventData, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ServiceInvocationException($"Failed to publish event to {pubsubName}/{topicName}.", ex);
        }
    }
}

/// <summary>
/// Exception thrown when service invocation fails.
/// </summary>
public class ServiceInvocationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ServiceInvocationException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ServiceInvocationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
