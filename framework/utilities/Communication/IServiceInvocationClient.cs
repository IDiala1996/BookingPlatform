namespace SmartServices.Framework.Utilities.Communication;

/// <summary>
/// Provides service-to-service communication using Dapr service invocation and pub/sub blocks.
/// Enables loose coupling between microservices.
/// </summary>
public interface IServiceInvocationClient
{
    /// <summary>
    /// Gets the name of the default publish-subscribe component used for event messaging.
    /// </summary>
    public string DefaultPubSubName { get;}
    /// <summary>
    /// Invokes a method on a remote service.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="serviceName">The name of the target service.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the remote service.</returns>
    Task<TResponse> InvokeAsync<TResponse>(
        string serviceName,
        string methodName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes a method on a remote service with request data.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="serviceName">The name of the target service.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="request">The request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the remote service.</returns>
    Task<TResponse> InvokeAsync<TRequest, TResponse>(
        string serviceName,
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes a method on a remote service.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="httpMethod">The httpMethod to use when invoking the target method.</param>
    /// <param name="serviceName">The name of the target service.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the remote service.</returns>
    Task<TResponse> InvokeAsync<TResponse>(
        HttpMethod httpMethod,
        string serviceName,
        string methodName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Invokes a method on a remote service with request data.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="httpMethod">The httpMethod to use when invoking the target method.</param>
    /// <param name="serviceName">The name of the target service.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    /// <param name="request">The request data.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the remote service.</returns>
    Task<TResponse> InvokeAsync<TRequest, TResponse>(
        HttpMethod httpMethod,
        string serviceName,
        string methodName,
        TRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an event to a topic.
    /// </summary>
    /// <param name="pubsubName">The name of the pub/sub component.</param>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="eventData">The event data to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishEventAsync<TEvent>(
        string pubsubName,
        string topicName,
        TEvent eventData,
        CancellationToken cancellationToken = default)
        where TEvent : class;

    /// <summary>
    /// Publishes an event to a topic.
    /// </summary>
    /// <param name="topicName">The name of the topic.</param>
    /// <param name="eventData">The event data to publish.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task PublishEventAsync<TEvent>(
        string topicName,
        TEvent eventData,
        CancellationToken cancellationToken = default)
        where TEvent : class;
}
