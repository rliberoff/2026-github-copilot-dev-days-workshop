---
applyTo: "**/Options/**/*.cs;**/*Options.cs"
description: "Guidelines for creating configuration options classes implementing the 'Options Pattern' which uses classes to provide strongly typed access to groups of related settings, following established patterns for validation, immutability, and binding."
---

# Options Classes Development Guidelines

Instructions for creating configuration options classes following established architectural patterns from the 'Options Pattern'.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Pattern**: Options pattern with `IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`
- **Validation**: Data Annotations with fail-fast validation on startup

## When to create an `Options` class

Create a dedicated options class when:

1. **External Configuration Required**: The service needs values from `appsettings.json`, environment variables, Azure App Configuration, or Key Vault
2. **Multiple Related Settings**: Three or more configuration values logically belong together
3. **Validation Needed**: Configuration values require validation (ranges, formats, required fields)
4. **Hot-Reload Desired**: The application should respond to configuration changes without restart
5. **Testability**: You want to inject different configurations for unit testing

**DO NOT create options classes for:**

- Single values that can be passed as constructor parameters
- Single connection strings or endpoints
- Compile-time constants that never change
- Values derived from other configuration (compute these in the service)

## Step-by-Step: Creating `Options` from a specification

When given a specification or requirement, follow this process:

### Step 1: Identify configuration values

From the specification, extract:

- **Connection strings or endpoints**: URLs, URIs, connection information
- **Credentials**: API keys, secrets, tokens (mark as sensitive)
- **Behavioral settings**: Timeouts, retry counts, batch sizes
- **Feature flags**: Enable/disable toggles
- **Resource identifiers**: Database names, container IDs, queue names

### Step 2: Determine property types

| Specification Says                    | Use Type               | Notes                                                                                                                    |
| ------------------------------------- | ---------------------- | ------------------------------------------------------------------------------------------------------------------------ |
| "URL", "endpoint", "address"          | `Uri`                  | Use `Uri` for type safety                                                                                                |
| "connection string"                   | `string`               | Keep as string for SDK compatibility                                                                                     |
| "timeout", "duration", "interval"     | `double` or `TimeSpan` | Prefer `double` for units like seconds, milliseconds or minutes. For any other cases use `TimeSpan`                      |
| "count", "limit", "size", "number of" | `int`                  | Use `Range` for bounds                                                                                                   |
| "percentage", "ratio", "probability"  | `double`               | Range 0.0 to 1.0                                                                                                         |
| "enable", "disable", "use", "allow"   | `bool`                 | Default based on safest option                                                                                           |
| "optional", "if provided", "when set" | nullable type `T?`     | No `required` keyword                                                                                                    |
| "list of", "multiple", "collection"   | `IReadOnlyList<T>`     | Immutable collection                                                                                                     |
| "one of", "mode", "type", "strategy"  | `enum`                 | Define enum, use `[EnumDataType]`                                                                                        |

### Step 3: Determine required vs optional

- **Required**: Use `required` keyword plus the `[Required]` attribute
- **Optional with default**: Omit `required`, provide sensible default
- **Optional nullable**: Use `T?` type, no `[Required]`

### Step 4: Apply Naming Conventions

| Element        | Convention                                                 | Example                          |
| -------------- | ---------------------------------------------------------- | -------------------------------- |
| Class name     | `{Feature}Options`                                         | `ServiceOptions`                 |
| Namespace      | `{Project}.Services.{Area}.Options` or `{Project}.Options` | `MyApp.Services.Storage.Options` |
| File name      | `{ClassName}.cs`                                           | `AzureStorageOptions.cs`         |
| Config section | Match class name                                           | `"AzureStorageOptions": { }`     |
| Property names | PascalCase, descriptive nouns                              | `MaxRetryCount`, `Endpoint`      |

### Step 5: Add validation constraints

Think about each property:

- What is the minimum valid value?
- What is the maximum valid value?
- What format must it follow (URL, email, regex)?
- Is empty or whitespace meaningful or invalid?
- Are there dependencies between properties?

## Worked Example: From specification to `Options` class

**Given specification:**

> "Create a service that integrates with a third-party weather API. It needs:
>
> - The API base URL (required)
> - An API key for authentication (required)
> - A timeout setting (default 30 seconds, max 2 minutes)
> - Option to enable response caching (default on)
> - Cache duration (default 5 minutes)
> - Maximum concurrent requests (default 10)
> - Optional proxy URL if corporate network requires it"

**Analysis:**

| Requirement                     | Property Name           | Type     | Required? | Validation                                                                                                           |
| ------------------------------- | ----------------------- | -------- | --------- | -------------------------------------------------------------------------------------------------------------------- |
| Timeout (30s default, max 120s) | `TimeoutSeconds`        | `double` | No        | `[Range(1, 120)]`                                                                                                    |
| Enable caching (default on)     | `EnableCaching`         | `bool`   | No        | None                                                                                                                 |
| Cache duration (default 5 min)  | `CacheDurationMinutes`  | `int`    | No        | `[Range(1, 60)]`                                                                                                     |
| Max concurrent requests         | `MaxConcurrentRequests` | `int`    | No        | `[Range(1, 100)]`                                                                                                    |

**Resulting `Options` class:**

```csharp
namespace MyProject.Services.Weather.Options;

/// <summary>
/// Configuration options for the Weather API integration.
/// </summary>
/// <remarks>
/// Bind to the `WeatherApiOptions` configuration section in `appsettings.json`.
/// </remarks>
public sealed class WeatherApiOptions
{
    /// <summary>
    /// Gets the base URL of the Weather API.
    /// </summary>
    /// <example>https://api.weather.example.com/v2</example>
    [Required]
    [Uri]
    public required Uri BaseUrl { get; init; }

    /// <summary>
    /// Gets the API key for authentication.
    /// </summary>
    /// <remarks>
    /// Obtain your API key from the Weather API developer portal.
    /// Store in Azure Key Vault for production deployments.
    /// </remarks>
    [Required]
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets the request timeout in seconds.
    /// </summary>
    /// <value>The default value is <c>30</c> seconds.</value>
    [Range(1, 120)]
    public double TimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// Gets a value indicating whether to cache API responses.
    /// </summary>
    /// <value>The default value is <c>true</c>.</value>
    public bool EnableCaching { get; init; } = true;

    /// <summary>
    /// Gets the cache duration in minutes.
    /// </summary>
    /// <value>The default value is <c>5</c> minutes.</value>
    /// <remarks>
    /// Only applies when <see cref="EnableCaching"/> is <c>true</c>.
    /// </remarks>
    [Range(1, 60)]
    public int CacheDurationMinutes { get; init; } = 5;

    /// <summary>
    /// Gets the maximum number of concurrent API requests.
    /// </summary>
    /// <value>The default value is <c>10</c>.</value>
    [Range(1, 100)]
    public int MaxConcurrentRequests { get; init; } = 10;

    /// <summary>
    /// Gets the proxy URL for corporate network environments.
    /// </summary>
    /// <value><c>null</c> if no proxy is required.</value>
    public Uri? ProxyUrl { get; init; }
}
```

**Corresponding appsettings.json:**

```json
{
  "WeatherApiOptions": {
    "BaseUrl": "https://api.weather.example.com/v2",
    "ApiKey": "your-api-key-here",
    "TimeoutSeconds": 30,
    "EnableCaching": true,
    "CacheDurationMinutes": 5,
    "MaxConcurrentRequests": 10,
    "ProxyUrl": null
  }
}
```

**Registration:**

```csharp
builder.Services.AddOptionsWithValidateOnStart<WeatherApiOptions>()
                .Bind(builder.Configuration.GetSection(nameof(WeatherApiOptions)))
                .ValidateDataAnnotations();
```

## Key Conventions

### Use `required` and `init` for immutability

```csharp
// GOOD - Immutable with required initialization
public required string ApiKey { get; init; }

// GOOD - Optional with default value
public int RetryCount { get; init; } = 3;

// AVOID - Mutable after initialization
public string ApiKey { get; set; } = string.Empty;
```

### Use `sealed` for leaf options

```csharp
// GOOD - Cannot be inherited
public sealed class AzureOpenAIOptions { }

// Use non-sealed only when inheritance is needed
public class BaseAgentOptions { }
public sealed class DaxHelperOptions : BaseAgentOptions { }
```

## Validation attributes

### Required attributes

```csharp
using System.ComponentModel.DataAnnotations;

public sealed class MyOptions
{
    // Required string that cannot be empty or whitespace
    [Required]
    public required string ConnectionString { get; init; }

    // Required numeric with range validation
    [Required]
    [Range(1, int.MaxValue)]
    public required int MaxTokens { get; init; }

    // Required URL validation
    [Required]
    public required string Endpoint { get; init; }

    // Required email validation
    [Required]
    public required string NotificationEmail { get; init; }
}
```

### Optional with defaults

```csharp
public sealed class ResilienceOptions
{
    // Optional with sensible default
    [Range(1, 86400)]
    public double AttemptTimeoutSeconds { get; init; } = 90;

    // Optional nullable for truly optional values
    public int? MaxConcurrency { get; init; }

    // Optional boolean with default
    public bool EnableCaching { get; init; } = true;
}
```

### Common validation patterns

| Scenario         | Attributes                               | Example            |
| ---------------- | ---------------------------------------- | ------------------ |
| Non-empty string | `[Required]`, `[NotEmptyOrWhitespace]`   | API keys, names    |
| Positive integer | `[Required]`, `[Range(1, int.MaxValue)]` | Counts, limits     |
| Percentage       | `[Range(0.0, 1.0)]`                      | Temperature, top_p |
| URL              | `[Required]`, `[Uri]`                    | Endpoints          |
| Timeout          | `[Range(1, 86400)]`                      | Seconds            |
| Enum             | `[Required]`, `[EnumDataType]`           | Service modes      |

## Nested `Options` pattern

For complex configurations, use nested classes:

```csharp
public sealed class AzureOpenAIOptions
{
    [Required]
    public required Uri Endpoint { get; init; }

    [Required]
    public required string Key { get; init; }

    [Required]
    public required string ChatModelDeploymentName { get; init; }

    // Nested options for token-based authentication
    public TokenCredentialsOptions? TokenCredentialsOptions { get; init; }

    public bool UseTokenCredentialAuthentication { get; init; }
}

public sealed class TokenCredentialsOptions
{
    [Required]
    public required string ClientId { get; init; }

    [Required]
    public required string ClientSecret { get; init; }

    [Required]
    public required string TenantId { get; init; }
}
```

## Interface-Based `Options`

For generic service configurations:

```csharp
// Define interface
public interface IQueriesCatalogOptions
{
    string DatasetId { get; }
}

// Implement in concrete options
public sealed class PerformanceAnalyzerOptions : IQueriesCatalogOptions
{
    [Required]
    [NotEmptyOrWhitespace]
    public required string DatasetId { get; init; }

    [Required]
    [Range(1, 100)]
    public required int TopNResults { get; init; }
}

// Use in generic service
public sealed class OperationCatalogService<TOptions> : IOperationCatalogService
    where TOptions : class, IQueriesCatalogOptions
{
    private TOptions options;

    public OperationCatalogService(IOptionsMonitor<TOptions> optionsMonitor)
    {
        options = optionsMonitor.CurrentValue;
        optionsMonitor.OnChange(newOptions => options = newOptions);
    }
}
```

## Registration patterns

### Basic registration

```csharp
builder.Services.AddOptionsWithValidateOnStart<MyOptions>()
                .Bind(builder.Configuration.GetSection(nameof(MyOptions)))
                .ValidateDataAnnotations();
```

### Named Options (Keyed)

```csharp
// Register multiple configurations with names
builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>(@"Primary")
                .Bind(builder.Configuration.GetSection(@"AzureOpenAI:Primary"))
                .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>(@"Secondary")
                .Bind(builder.Configuration.GetSection(@"AzureOpenAI:Secondary"))
                .ValidateDataAnnotations();

// Use with IOptionsSnapshot or IOptionsMonitor
public class MyService
{
    private readonly AzureOpenAIOptions primaryOptions;

    public MyService(IOptionsSnapshot<AzureOpenAIOptions> optionsSnapshot)
    {
        primaryOptions = optionsSnapshot.Get(@"Primary");
    }
}
```

### Post-Configure for complex initialization

```csharp
builder.Services.AddOptionsWithValidateOnStart<PowerBIOptions>()
                .Bind(builder.Configuration.GetSection(nameof(PowerBIOptions)))
                .Configure(options =>
                {
                    // Complex initialization from JSON string
                    var headersJson = builder.Configuration[@"PowerBIOptions:Headers"];
                    if (!string.IsNullOrWhiteSpace(headersJson))
                    {
                        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson)!;
                        options.UseValues(headers);
                    }
                })
                .ValidateDataAnnotations();
```

## Hot-Reload pattern

Use `IOptionsMonitor<T>` for configuration changes at runtime:

```csharp
public class MyService
{
    private MyOptions options;

    public MyService(IOptionsMonitor<MyOptions> optionsMonitor)
    {
        options = optionsMonitor.CurrentValue;

        // Subscribe to changes
        optionsMonitor.OnChange(newOptions =>
        {
            options = newOptions;
            OnOptionsChanged(newOptions);
        });
    }

    private void OnOptionsChanged(MyOptions newOptions)
    {
        // Handle configuration changes
        logger.LogInformation(@"Options updated: MaxRetries = {MaxRetries}", newOptions.MaxRetries);
    }
}
```

### When to use each interface

| Interface             | Lifetime  | Use Case                                |
| --------------------- | --------- | --------------------------------------- |
| `IOptions<T>`         | Singleton | Static configuration that never changes |
| `IOptionsSnapshot<T>` | Scoped    | Per-request configuration in web apps   |
| `IOptionsMonitor<T>`  | Singleton | Long-lived services needing hot-reload  |

## Defaults via constants

Use shared constants for default values:

```csharp
// In Constants.cs or SettingsConstants.cs
internal static class SettingsConstants
{
    internal static class TableNames
    {
        internal const string IntentCategories = @"IntentCategories";
        internal const string UserProfiles = @"UserProfiles";
    }

    internal static class Defaults
    {
        internal const int MaxRetries = 3;
        internal const int MaxTokens = 4096;
        internal const double Temperature = 1.0;
    }
}

// In Options class
public sealed class MyOptions
{
    public string TableName { get; init; } = SettingsConstants.TableNames.IntentCategories;

    public int MaxRetries { get; init; } = SettingsConstants.Defaults.MaxRetries;
}
```

## XML documentation

Always document options properties with clear, actionable descriptions:

```csharp
/// <summary>
/// Configuration options for Azure OpenAI service integration.
/// </summary>
/// <remarks>
/// Bind to the "AzureOpenAIOptions" configuration section.
/// </remarks>
public sealed class AzureOpenAIOptions
{
    /// <summary>
    /// Gets the Azure OpenAI service endpoint URL.
    /// </summary>
    /// <example>https://my-openai.openai.azure.com/</example>
    [Required]
    public required Uri Endpoint { get; init; }

    /// <summary>
    /// Gets the API key for authentication.
    /// Required when <see cref="UseTokenCredentialAuthentication"/> is <c>false</c>.
    /// </summary>
    public string? Key { get; init; }

    /// <summary>
    /// Gets a value indicating whether to use token-based authentication instead of API key authentication.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, <see cref="TokenCredentialsOptions"/> must be configured or <see cref="DefaultAzureCredential"/> will be used.
    /// </remarks>
    public bool UseTokenCredentialAuthentication { get; init; }
}
```

### Documentation guidelines

For each property, the XML documentation should answer:

1. **What is it?** - Clear description of the setting's purpose
2. **What format?** - Expected format, units (seconds, milliseconds, bytes)
3. **What's the default?** - Document default values if any
4. **What happens if?** - Behavior when set to specific values
5. **Dependencies** - Related properties that affect behavior

Use these documentation patterns:

```csharp
/// <summary>
/// Gets the maximum time in seconds to wait for a response.
/// </summary>
/// <value>The default value is <c>30</c> seconds.</value>
/// <remarks>
/// Set to <c>0</c> for no timeout. Values greater than <c>300</c> may cause gateway timeouts.
/// </remarks>
[Range(0, 300)]
public double TimeoutSeconds { get; init; } = 30;

/// <summary>
/// Gets the list of allowed origins for CORS.
/// </summary>
/// <value>An empty list allows no origins. Use <c>["*"]</c> to allow all origins (not recommended for production).</value>
public IReadOnlyList<string> AllowedOrigins { get; init; } = [];
```

## File organization

- Place options in `Options/` folder within the project.
- One options class per file.
- File name matches class name: `MyOptions.cs`.
- Group related options in the same namespace.

```text
Project/
├── Options/
│   ├── AgentOptions.cs
│   ├── ResilienceOptions.cs
│   └── TelemetryOptions.cs
```

## `appsettings.json` Structure

Options should map to JSON sections:

```json
{
  "AzureOpenAIOptions": {
    "Endpoint": "https://my-openai.openai.azure.com/",
    "Key": "your-api-key",
    "ChatModelDeploymentName": "gpt-4o",
    "UseTokenCredentialAuthentication": false,
    "TokenCredentialsOptions": {
      "ClientId": "client-id",
      "ClientSecret": "client-secret",
      "TenantId": "tenant-id"
    }
  },
  "ResilienceOptions": {
    "AttemptTimeoutSeconds": 90,
    "TotalRequestTimeoutSeconds": 270
  }
}
```

## Advanced patterns

### Conditional validation

For properties that are required only when another property has a specific value:

```csharp
using System.ComponentModel.DataAnnotations;

public sealed class MyOptions : IValidatableObject
{
    public Mode Mode { get; init; } = Mode.A;

    public string? Value { get; init; }

    public SomeOptions? Some { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (Mode == Mode.A && string.IsNullOrWhiteSpace(Value))
        {
            yield return new ValidationResult(@"Value is required when Mode is A", [nameof(Value)]);
        }

        if (Mode == Mode.B && Some is null)
        {
            yield return new ValidationResult(@"Some is required when Mode is B", [nameof(Some)]);
        }
    }
}

public enum Mode
{
    A,
    B,
    C
}
```

### Fluent configuration methods

For options that need complex initialization:

```csharp
public sealed class HttpClientOptions
{
    private readonly Dictionary<string, string> headers = [];

    [Required]
    public required Uri BaseAddress { get; init; }

    public IReadOnlyDictionary<string, string> DefaultHeaders => headers;

    /// <summary>
    /// Adds headers from an external source (e.g., deserialized JSON).
    /// </summary>
    public void UseHeaders(IDictionary<string, string> additionalHeaders)
    {
        foreach (var (key, value) in additionalHeaders)
        {
            headers[key] = value;
        }
    }
}
```

## Validation error messages

Provide custom error messages for better developer experience:

```csharp
public sealed class ApiOptions
{
    [Required(ErrorMessage = @"Endpoint is required. Provide the full URL to the API.")]
    public required Uri Endpoint { get; init; }

    [Required(ErrorMessage = @"ApiKey is required for authentication.")]
    [MinLength(32, ErrorMessage = @"ApiKey must be at least 32 characters.")]
    public required string ApiKey { get; init; }

    [Range(1, 300, ErrorMessage = @"TimeoutSeconds must be between 1 and 300.")]
    public double TimeoutSeconds { get; init; } = 30;
}
```

## Security considerations

### Sensitive data handling

Never hardcode secrets in options classes. Use these patterns:

```csharp
public sealed class SecureServiceOptions
{
    /// <summary>
    /// Gets the API key.
    /// </summary>
    /// <remarks>
    /// Store in Azure Key Vault and reference via configuration:
    /// <code>"ApiKey": "@Microsoft.KeyVault(SecretUri=https://myvault.vault.azure.net/secrets/api-key)"</code>
    /// </remarks>
    [Required]
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    /// <remarks>
    /// Use managed identity where possible. If connection string is required, store in Azure Key Vault.
    /// </remarks>
    [Required]
    public required string ConnectionString { get; init; }
}
```

### Configuration sources priority

Document the expected configuration sources:

```csharp
/// <summary>
/// Options for database connectivity.
/// </summary>
/// <remarks>
/// <para>Configuration is loaded from (in order of precedence):</para>
/// <list type="number">
///   <item>Environment variables</item>
///   <item>Azure App Configuration</item>
///   <item>Azure Key Vault (for secrets)</item>
///   <item>appsettings.{Environment}.json</item>
///   <item>appsettings.json</item>
/// </list>
/// </remarks>
public sealed class DatabaseOptions { }
```

### Avoid logging sensitive options

Mark sensitive properties to prevent accidental logging:

```csharp
public sealed class AuthOptions
{
    [Required]
    [DataType(DataType.Password)]  // Hints that this is sensitive
    public required string ClientSecret { get; init; }

    // Override ToString to exclude sensitive data
    public override string ToString() => $"AuthOptions {{ ClientId = {ClientId}, TenantId = {TenantId} }}";
}
```

## Options class checklist

Before finalizing an options class, verify:

- [ ] **Class is `sealed`** unless inheritance is explicitly needed
- [ ] **Properties use `required init`** for mandatory configuration
- [ ] **Properties use `init`** (not `set`) for immutability
- [ ] **All properties have XML documentation** with clear descriptions
- [ ] **`[Required]` attribute** on mandatory properties
- [ ] **`[Range]`** on numeric properties with valid bounds
- [ ] **Sensible defaults** for optional properties
- [ ] **Namespace follows convention** `{Project}.Services.{Area}.Options`
- [ ] **Configuration section name** matches class name
- [ ] **Registration uses `AddOptionsWithValidateOnStart`** for fail-fast behavior
- [ ] **appsettings.json example** provided or documented
- [ ] **Sensitive data** is sourced from Key Vault, not hardcoded
- [ ] **Custom error messages** on validation attributes for developer clarity

## References

- [Options pattern in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options)

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
