namespace DragonBallLibrary.ServiceDefaults.Services;

public interface IOutputBindingService
{
    Task PublishMessageAsync<T>(T message, string bindingName, CancellationToken cancellationToken);
    Task PublishMessageAsync<T>(T message, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
    Task PublishMessageAsync<T>(T message, string operation, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
    Task TryPublishMessageAsync<T>(T message, string bindingName, CancellationToken cancellationToken);
    Task TryPublishMessageAsync<T>(T message, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);
    Task TryPublishMessageAsync<T>(T message, string operation, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken);

}

public class OutputBindingService(DaprClient client, ILogger<OutputBindingService> logger) : IOutputBindingService
{
    public async Task PublishMessageAsync<T>(T message, string bindingName, CancellationToken cancellationToken) =>
        await PublishMessageAsync(message, null, bindingName, cancellationToken);

    public async Task PublishMessageAsync<T>(T message, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken) =>
       await PublishMessageAsync(message, "create", metadata, bindingName, cancellationToken);

    public async Task PublishMessageAsync<T>(T message, string operation, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing message {Message} to {BindingName}", message, bindingName);

        await client.InvokeBindingAsync(bindingName, operation, message, metadata, cancellationToken);
    }

    public async Task TryPublishMessageAsync<T>(T message, string bindingName, CancellationToken cancellationToken) =>
        await TryPublishMessageAsync(message, null, bindingName, cancellationToken);

    public async Task TryPublishMessageAsync<T>(T message, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken) =>
       await TryPublishMessageAsync(message, "create", metadata, bindingName, cancellationToken);

    public async Task TryPublishMessageAsync<T>(T message, string operation, IReadOnlyDictionary<string, string>? metadata, string bindingName, CancellationToken cancellationToken)
    {
        try
        {
            await PublishMessageAsync(message, operation, metadata, bindingName, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing message {Message} to {BindingName}: {MessageException}", message, bindingName, ex.Message);
        }
    }

}
