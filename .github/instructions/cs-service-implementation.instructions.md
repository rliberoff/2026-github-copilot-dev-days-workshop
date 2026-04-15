---
applyTo: "**/Services/**/*.cs;**/Abstractions/**/*.cs"
description: "Guidelines for implementing service classes and interfaces following established patterns for dependency injection, factory methods, and result handling."
---

# Service implementation guidelines

Instructions for implementing service classes following established architectural patterns.

## Project context

- **Target Framework**: .NET 10 with C# 14
- **Pattern**: Interface-first design with dependency injection
- **Error Handling**: Result pattern for clean error propagation

## Interface-first design

Always define a `public` interface in `{ProjectName}.Abstractions` before implementing a service. Include XML doc comments on the interface.

```csharp
namespace {ProjectName}.Abstractions;

public interface I{ServiceName}Service
{
    Task<Item?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<Item>> CreateAsync(Item item, CancellationToken cancellationToken);
}
```

## Implementations must be `internal sealed`

**Only abstractions are contracts. Implementations are details.**

- **`internal`** — prevents accidental public contracts, enforces dependency inversion (consumers depend on interfaces only), and enables safe refactoring without cross-assembly breakage.
- **`sealed`** — prevents undocumented inheritance contracts, avoids fragile base class problems, and enables compiler devirtualization/inlining.

```csharp
// GOOD
internal sealed class OrderService : IOrderService { }

// BAD — leaks implementation as API surface
public class OrderService : IOrderService { }
```

### Exceptions to `sealed`

Omit `sealed` **only** when:

1. The type is an explicit framework extension point or SDK base for third-party inheritance
2. A documented abstract base class for plugins
3. Inheritance demonstrably improves reusability — the base provides substantial shared implementation, multiple concrete types benefit, and virtual extension points are documented

**When uncertain:** ask the user, review the spec, default to `sealed`, prefer composition over inheritance. In exception cases, prefer `public abstract` over `public` concrete, and treat virtual methods as stable contracts.

## Service implementation pattern

```csharp
namespace {ProjectName}.Services;

internal sealed class {ServiceName}Service : I{ServiceName}Service
{
    private readonly ILogger<{ServiceName}Service> logger;
    private readonly TelemetryClient telemetryClient;
    private {ServiceName}Options options;

    public {ServiceName}Service(
        ILogger<{ServiceName}Service> logger,
        TelemetryClient telemetryClient,
        IOptionsMonitor<{ServiceName}Options> optionsMonitor)
    {
        this.logger = logger;
        this.telemetryClient = telemetryClient;
        options = optionsMonitor.CurrentValue;
        optionsMonitor.OnChange(newOptions => options = newOptions); // hot-reload
    }

    public async Task<Result<Item>> CreateAsync(Item item, CancellationToken cancellationToken)
    {
        logger.LogInformation(@"Creating item {ItemId}", item.Id);
        try
        {
            await SaveItemAsync(item, cancellationToken);
            telemetryClient.TrackEvent(@"ItemCreated", new Dictionary<string, string>
            {
                [@"ItemId"] = item.Id,
            });
            return Result<Item>.Ok(item);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, @"Error creating item {ItemId}", item.Id);
            return Result<Item>.Fail(ex);
        }
    }
}
```

## Guard clauses — `CommunityToolkit.Diagnostics`

Use `Guard` for parameter validation. **Always use the latest package version.**

```xml
<PackageReference Include="CommunityToolkit.Diagnostics" Version="8.4.0" />
```

```csharp
Guard.IsNotNull(item);
Guard.IsNotNullOrWhiteSpace(userId);
Guard.IsGreaterThan(request.Timeout, 0);
Guard.IsNotEmpty(collection);
```

Refer to `CommunityToolkit.Diagnostics.Guard` API docs for the full method list (null, string, numeric, collection, boolean, and type checks).

## Result pattern

Use for operations that can fail. **Never add implicit operators** — they silently convert `null` into `Ok(null)`.

```csharp
namespace {ProjectName}.Services;

public class Result
{
    protected Result(bool isSuccess, string? error = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public Exception? Exception { get; }

    public static Result Ok() => new(true);
    public static Result Fail(string error) => new(false, error);
    public static Result Fail(Exception exception) => new(false, exception.Message, exception);
}

public class Result<T> : Result
{
    protected Result(bool isSuccess, T? value = default, string? error = null, Exception? exception = null)
        : base(isSuccess, error, exception) => Value = value;

    public T? Value { get; }

    public static Result<T> Ok(T value) => new(true, value);
    public static new Result<T> Fail(string error) => new(false, default, error);
    public static new Result<T> Fail(Exception exception) => new(false, default, exception.Message, exception);
}
```

## Service configuration record

Bundle many dependencies into a single `record` when constructor parameter count grows large:

```csharp
public sealed record {ServiceName}ServiceConfiguration(
    DaprClient DaprClient,
    IDistributedCache DistributedCache,
    IHttpClientFactory HttpClientFactory,
    ILogger<{ServiceName}Service> Logger,
    IOptionsMonitor<{ServiceName}Options> ServiceOptions);
```

## Factory pattern for client creation

Use a `static` factory class with `Guard` validation, `ChainedTokenCredential` (service principal + `DefaultAzureCredential` fallback), and configurable client options (retry, timeout, headers). Separate `ConfigureClientOptions`, `CreateClientInstance`, and `BuildCredentialChain` into private methods.

```csharp
public static class {ClientName}Factory
{
    public static TClient CreateClient<TClient, TOptions>(TOptions options)
        where TClient : class where TOptions : class
    {
        Guard.IsNotNull(options);
        var clientOptions = ConfigureClientOptions(options);
        return options.UseTokenAuthentication
            ? new TClient(options.Endpoint, BuildCredentialChain(options), clientOptions)
            : new TClient(options.Endpoint, new ApiKeyCredential(options.ApiKey), clientOptions);
    }
}
```

## Factory function registration

Register `Func<…>` delegates as singletons for per-instance creation:

```csharp
services.AddSingleton<Func<string, CancellationToken, Task<IChatMessageStore>>>(sp =>
{
    var opts = sp.GetRequiredService<IOptionsMonitor<CosmosOptions>>().CurrentValue;
    return async (conversationId, ct) =>
    {
        var client = new CosmosClient(opts.ConnectionString);
        var db = await client.CreateDatabaseIfNotExistsAsync(opts.Database, cancellationToken: ct);
        await db.Database.CreateContainerIfNotExistsAsync(containerId, "/id", cancellationToken: ct);
        return new CosmosChatMessageStore(client, opts.Database, containerId, conversationId);
    };
});
```

## Warm-up hosted service

Implement `IHostedService` to pre-load data at startup. Log success and catch/warn on failure so the app still starts.

```csharp
internal sealed class {ServiceName}WarmUpHostedService(
    I{ServiceName}Service service,
    ILogger<{ServiceName}WarmUpHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try { await service.InitializeAsync(cancellationToken); }
        catch (Exception ex) { logger.LogWarning(ex, @"Warm-up failed; will init on first use"); }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```

## Semaphore service for rate limiting

Use `SemaphoreSlim` with `IOptionsMonitor` hot-reload and a `lock` to swap the semaphore when the limit changes. Expose `WaitAsync`/`Release`.

## Service lifetime selection

### Decision matrix

| Question                        | Choose                     |
| ------------------------------- | -------------------------- |
| Per-request state or resources? | Scoped                     |
| Global coordination or cache?   | Singleton                  |
| Stateless, cheap computation?   | Transient                  |
| Disposable resources?           | Scoped                     |
| Must be thread-safe?            | Singleton                  |
| Transaction boundaries?         | Scoped                     |
| Database context?               | Scoped                     |
| Background processing?          | Singleton (Hosted Service) |

### Allowed dependency directions

| Lifetime  | Can depend on     |
| --------- | ----------------- |
| Singleton | Singleton only    |
| Scoped    | Scoped, Singleton |
| Transient | Any               |

**Forbidden:** Singleton → Scoped (captive dependency). Singleton → Transient that captures scoped state.

### Registration examples

```csharp
builder.Services.AddHostedService<{ServiceName}WarmUpHostedService>();
builder.Services.AddSingleton<IDistributedCache, RedisCache>();
builder.Services.AddScoped<I{ServiceName}Service, {ServiceName}Service>();
builder.Services.AddTransient<IValidator<Order>, OrderValidator>();
```

> **When in doubt, prefer the simpler approach, ask the user or request better specifications.**
