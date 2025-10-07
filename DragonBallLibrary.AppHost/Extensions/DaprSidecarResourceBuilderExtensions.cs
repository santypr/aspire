using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;

namespace DragonBallLibrary.AppHost.Extensions;
public static class DaprSidecarResourceBuilderExtensions
{
  private const string ConnectionStringEnvironmentName = "ConnectionStrings__";

  public static IResourceBuilder<T> WithCustomDaprSidecar<T>(this IResourceBuilder<T> builder, string appId, Dictionary<string, string?>? envVars) where T : IResource
  {
    return builder.WithDaprSidecar(o =>
     {
       o.WithOptions(new DaprSidecarOptions
       {
         AppId = appId,
         ResourcesPaths = ImmutableHashSet.Create("../dapr/components"),
         Config = Path.Combine("..", "dapr", "components", "config.yaml")
       });

       if (envVars is not null)
       {
           envVars
               .Where(envVar => !string.IsNullOrEmpty(envVar.Value))
               .ToList()
               .ForEach(envVar => o.WithEnvironment(envVar.Key, envVar.Value));
       }
     });
  }

  public static IResourceBuilder<IDaprSidecarResource> WithReference(this IResourceBuilder<IDaprSidecarResource> builder, IResourceBuilder<IResourceWithConnectionString> component, string? connectionName = null, bool optional = false)
  {
    connectionName ??= component.Resource.Name;

    builder.WithAnnotation(new EnvironmentCallbackAnnotation(context =>
    {
      var connectionStringName = component.Resource.ConnectionStringEnvironmentVariable ?? $"{ConnectionStringEnvironmentName}{connectionName}";
      context.EnvironmentVariables[connectionStringName] = new ConnectionStringReference(component.Resource, optional);
      return Task.CompletedTask;
    }));

    return builder;
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string name, string? value)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(name, () => value ?? string.Empty));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string name, ReferenceExpression value)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(context =>
    {
      context.EnvironmentVariables[name] = value;
      return Task.CompletedTask;
    }));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string name, Func<string> callback)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(name, callback));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, Action<EnvironmentCallbackContext> callback)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(callback));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, Func<EnvironmentCallbackContext, Task> callback)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(callback));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string name, EndpointReference endpointReference)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(context =>
    {
      context.EnvironmentVariables[name] = endpointReference;
      return Task.CompletedTask;
    }));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string name, IResourceBuilder<ParameterResource> parameter)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(context =>
    {
      context.EnvironmentVariables[name] = parameter.Resource;
      return Task.CompletedTask;
    }));
  }

  public static IResourceBuilder<IDaprSidecarResource> WithEnvironment(this IResourceBuilder<IDaprSidecarResource> builder, string envVarName, IResourceBuilder<IResourceWithConnectionString> resource)
  {
    return builder.WithAnnotation(new EnvironmentCallbackAnnotation(context =>
    {
      context.EnvironmentVariables[envVarName] = new ConnectionStringReference(resource.Resource, optional: false);
      return Task.CompletedTask;
    }));
  }
}
