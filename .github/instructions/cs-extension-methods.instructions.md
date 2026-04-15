---
applyTo: "**/Extensions/**/*.cs;**/*Extensions.cs"
description: "Guidelines for creating extension methods following established patterns for service registration, fluent APIs, and domain utilities in C#."
---

# Extension Methods Development Guidelines

Instructions for creating extension methods following established architectural patterns in C#.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Convention**: Extensions are discoverable and follow fluent API patterns
- **Namespace Strategy**: Use Microsoft namespaces for Dependency Injection extensions to enable auto-discovery.

## Parameter Validation (Mandatory)

All public extension methods **must** validate the `this` parameter using `ArgumentNullException.ThrowIfNull` as the first statement. This is a mandatory pattern to ensure fail-fast behavior and clear error messages.

```csharp
public static IServiceCollection AddMyService(this IServiceCollection services)
{
    ArgumentNullException.ThrowIfNull(services);

    // Implementation
    return services;
}
```

### Validation Rules

- **Always validate the `this` parameter** in public extension methods
- **Validate other non-nullable reference parameters** that are required for the method to function
- **Use `ArgumentNullException.ThrowIfNull`** (not manual null checks with `throw new ArgumentNullException`)
- **Place validation at the method entry point**, before any other logic
- **Include `<exception cref="ArgumentNullException">` in XML documentation** for validated parameters

### Multi-Parameter Validation Example

```csharp
public static IServiceCollection AddMyService(this IServiceCollection services, IConfiguration configuration, string sectionName)
{
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);
    ArgumentException.ThrowIfNullOrWhiteSpace(sectionName);

    // Implementation
    return services;
}
```

### Private Implementation Methods

When using the multiple overloads pattern, validation should occur in each public method before delegating to the private implementation. The private `Inner*` method does not need redundant validation:

```csharp
// Public method validates
public static IServiceCollection AddMyService(this IServiceCollection services)
{
    ArgumentNullException.ThrowIfNull(services);
    return InnerAddMyService(services, null);
}

// Private method trusts callers (all are internal)
private static IServiceCollection InnerAddMyService(IServiceCollection services, string? key)
{
    // No redundant validation needed
    // Implementation
    return services;
}
```

## Namespace Conventions

### Service Collection Extensions

Place extensions in Microsoft namespaces for automatic discoverability:

```csharp
// GOOD - Auto-discoverable without explicit using statement
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMyService(this IServiceCollection services)
    {
        // Implementation
        return services;
    }
}
```

```csharp
// GOOD - For host builder extensions
namespace Microsoft.Extensions.Hosting;

public static class IHostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddMyService(this IHostApplicationBuilder builder, string assemblyName)
    {
        // Implementation
        return builder;
    }
}
```

### Domain-Specific Extensions

Place in the domain namespace for explicit import:

```csharp
namespace {ProjectName}.Services.Extensions;

public static class ChatMessageContentExtensions
{
    public static bool HasReachedTokenLimit(this ChatMessageContent content)
    {
        // Implementation
    }
}
```

## Fluent API Pattern

Unless specified, all extension methods must return the extended type for chaining:

```csharp
public static IServiceCollection AddManager(this IServiceCollection services, IConfiguration configuration)
{
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    services.AddManager(configuration)
            .AddSingleton<IManager, Manager>();

    return services;  // Enable chaining
}
```

### Usage Pattern

```csharp
builder.Services
       .AddHttpClient()
       .AddDistributedMemoryCache()
       .AddManager(builder.Configuration)
       .AddHealthChecks();
```

## Multiple Overloads Pattern

Provide multiple overloads that delegate to a single private implementation:

```csharp
public static class IServiceCollectionExtensions
{
    // Minimal overload
    public static IServiceCollection AddMyService(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        return InnerAddMyService(services, null, null, lifetime);
    }

    // Keyed overload
    public static IServiceCollection AddMyService(
        this IServiceCollection services,
        string? key,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        return InnerAddMyService(services, key, null, lifetime);
    }

    // Action overload
    public static IServiceCollection AddMyService(
        this IServiceCollection services,
        Action<MyType>? myTypeActions,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        return InnerAddMyService(services, null, (myType, _) => myTypeActions?.Invoke(myType), lifetime);
    }

    // Full overload with service provider access
    public static IServiceCollection AddMyService(
        this IServiceCollection services,
        Action<MyType, IServiceProvider>? myTypeActions,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(services);
        return InnerAddMyService(services, null, myTypeActions, lifetime);
    }

    // Single private implementation (no redundant validation)
    private static IServiceCollection InnerAddMyService(
        IServiceCollection services,
        string? key,
        Action<MyType, IServiceProvider>? myTypeActions,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        // Full implementation here
        return services;
    }
}
```

## Factory Registration Pattern

For services that need per-instance creation:

```csharp
public static IServiceCollection AddServiceFactory(this IServiceCollection services, string containerId)
{
    ArgumentNullException.ThrowIfNull(services);
    ArgumentException.ThrowIfNullOrWhiteSpace(containerId);

    services.AddSingleton<Func<string, Task<ServiceStore>>>(serviceProvider =>
    {
        var serviceOptions = serviceProvider.GetRequiredService<IOptionsMonitor<ServiceOptions>>().CurrentValue;
        var connectionString = $"AccountEndpoint={serviceOptions.Endpoint};AccountKey={serviceOptions.AuthKey};";

        return async conversationId =>
        {
            var serviceClient = new ServiceClient(connectionString);
            var databaseResponse = await serviceClient.CreateDatabaseIfNotExistsAsync(serviceOptions.Database);
            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerId, "/id");

            return new ServiceStore(serviceClient, serviceOptions.Database, containerId, conversationId);
        };
    });

    return services;
}
```

Usage:

```csharp
// In service
private readonly Func<string, Task<ServiceStore>> serviceStoreFactory;

public async Task<ServiceStore> GetServiceStoreAsync(string conversationId)
{
    return await serviceStoreFactory(conversationId);
}
```

## Options Binding Extensions

Create extensions for common options patterns:

```csharp
public static IServiceCollection AddMyServiceWithOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string? sectionName = null) where TOptions : class
{
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    services.AddOptionsWithValidateOnStart<TOptions>()
            .Bind(configuration.GetSection(sectionName ?? typeof(TOptions).Name))
            .ValidateDataAnnotations();

    services.AddSingleton<IMyService, MyService>();

    return services;
}
```

## Configuration Extensions

```csharp
namespace Microsoft.Extensions.Configuration;

public static class IConfigurationExtensions
{
    public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var value = configuration.GetValue<T>(key);

        if (value is null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            throw new MissingConfigurationException($@"Missing required configuration: '{key}'");
        }

        return value;
    }

    public static string GetRequiredConnectionString(this IConfiguration configuration, string name)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var connectionString = configuration.GetConnectionString(name);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new MissingConfigurationException($@"Missing connection string: '{name}'");
        }

        return connectionString;
    }
}
```

## Naming Conventions

| Extension Target          | Class Name                          | File Name                              |
| ------------------------- | ----------------------------------- | -------------------------------------- |
| `IServiceCollection`      | `IServiceCollectionExtensions`      | `IServiceCollectionExtensions.cs`      |
| `IHostApplicationBuilder` | `IHostApplicationBuilderExtensions` | `IHostApplicationBuilderExtensions.cs` |
| `IConfiguration`          | `IConfigurationExtensions`          | `IConfigurationExtensions.cs`          |
| `string`                  | `StringExtensions`                  | `StringExtensions.cs`                  |
| `ChatMessageContent`      | `ChatMessageContentExtensions`      | `ChatMessageContentExtensions.cs`      |
| Domain type `Foo`         | `FooExtensions`                     | `FooExtensions.cs`                     |

## XML Documentation

Always include XML documentation on public extension methods:

```csharp
/// <summary>
/// Adds the Manager services to the service collection.
/// </summary>
/// <param name="services">The service collection to add services to.</param>
/// <param name="managerActions">Optional actions to configure the manager.</param>
/// <param name="lifetime">The service lifetime for the manager. Defaults to Singleton.</param>
/// <returns>The service collection for chaining.</returns>
/// <exception cref="ArgumentNullException">Thrown when services is null.</exception>
public static IServiceCollection AddManager( this IServiceCollection services, Action<Manager>? managerActions = null, ServiceLifetime lifetime = ServiceLifetime.Singleton)
{
    ArgumentNullException.ThrowIfNull(services);
    return InnerAddManager(services, null, managerActions, lifetime);
}
```

## Static Class Conventions

- Use `static class` for all extension containers
- Keep extension classes focused on a single type or domain
- Use `this` parameter modifier as the first parameter
- Mark internal helpers as `private static`

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
