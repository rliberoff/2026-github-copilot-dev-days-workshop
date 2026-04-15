---
applyTo: "**/Program.cs"
description: "Guidelines for creating Program.cs files for ASP.NET, Aspire and .NET applications, following conventions for configuration, telemetry, and dependency injection."
---

# Program.cs Guidelines

Instructions for creating well-structured `Program.cs` files for ASP.NET, background services, Aspire and .NET applications following established patterns and conventions.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Infrastructure**: Azure PaaS (App Configuration, Key Vault, Application Insights)
- **Distributed Systems**: Dapr sidecar integration, Aspire orchestration when required
- **Combine with instructions**: Follow these guidelines alongside the next other instructions:
  - [Options Classes Instructions](/.github/instructions/cs-options-classes.instructions.md) for configuration management.
  - [.NET Aspire Instructions](/.github/instructions/cs-aspire-apphost.instructions.md) for Aspire-specific patterns.

## Required File Structure

Every `Program.cs` file must follow a consistent section-based structure with clear comment headers:

1. Using Directives (grouped and ordered)
2. Assembly Name Resolution
3. Builder Creation
4. Configuration Loading
5. Telemetry Configuration
6. Options Registration
7. Application Services
8. Application Middleware Configuration
9. Application Execution

## Section-by-Section Guidelines

### 1. Using Directives

- Group using directives by category with blank lines between groups
- Order: System → Third-party → Project namespaces
- Use explicit namespaces for clarity at the entry point

```csharp
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;

using Azure.Identity;

using Company.Project;
using Company.Project.Services;
using Company.Project.Options;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;
```

### 2. Assembly Name Resolution

- Always resolve assembly name at the start for consistent telemetry and configuration labeling

```csharp
var assemblyName = typeof(Program).Assembly.GetName().Name;
```

### 3. Builder Creation

#### Web Applications (APIs, Agents, Bot)

```csharp
var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
```

#### Background Services (Jobs, Workers)

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
```

#### Aspire Application Host

```csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
```

### 4. Configuration Loading

Use structured section comments `/* Load Configuration */` and follow this exact pattern:

```csharp
/* Load Configuration */

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Load configuration from Azure App Configuration, and set Key Vault client for secrets...
var appConfigurationConnectionString = builder.Configuration.GetConnectionString(@"AppConfig");

var useAppConfiguration = !string.IsNullOrWhiteSpace(appConfigurationConnectionString);

if (useAppConfiguration)
{
    var azureCredentials = new ChainedTokenCredential(new DefaultAzureCredential(), new EnvironmentCredential());

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(appConfigurationConnectionString)
               .ConfigureKeyVault(keyVault =>
               {
                   keyVault.SetCredential(azureCredentials);
               })
               .Select(KeyFilter.Any, LabelFilter.Null) // Load configuration values with no label
               .Select(KeyFilter.Any, assemblyName) // Override with app-specific configuration
               .ConfigureRefresh(refreshOptions =>
               {
                   refreshOptions.Register(@"Sentinel", assemblyName, refreshAll: true);
                   refreshOptions.SetRefreshInterval(TimeSpan.FromSeconds(86400)); // Set to a day
               });
    }, optional: false);

    builder.Services.AddAzureAppConfiguration();
}
```

#### Configuration Loading Order (Priority)

1. `appsettings.json` (base configuration)
2. `appsettings.debug.json` (debugger-attached only)
3. `appsettings.{Environment}.json` (environment-specific)
4. `appsettings.{Username}.json` (developer-specific)
5. Environment variables
6. Azure App Configuration (with label hierarchy)

#### Environment Flags

```csharp
var isDevelopment = builder.Environment.IsDevelopment();
var isStaging = builder.Environment.IsStaging(); // Usually for testing purposes...
```

### 5. Telemetry Configuration

Use structured comment `/* Telemetry Configuration */` and follow this pattern:

```csharp
/* Telemetry Configuration */

// Configure OpenTelemetry for traces, metrics, and logs (via AddServiceDefaults)
// Application Insights TelemetryClient is retained for custom events (TrackEvent)
builder.AddServiceDefaults(assemblyName);

// TelemetryInitializers for Application Insights TelemetryClient custom events
builder.Services.AddVersionTelemetry();
builder.Services.AddAssemblyNameTelemetry(assemblyName ?? string.Empty);

builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = @"yyyy-MM-dd HH:mm:ss ";
    options.UseUtcTimestamp = true;
});

if (Debugger.IsAttached)
{
    builder.Logging.AddDebug();
}
```

### 6. Options Registration

Use structured comment `/* Load Options */` and follow this pattern:

```csharp
/* Load Options */

builder.Services.AddOptionsWithValidateOnStart<AzureOpenAIOptions>()
    .Bind(builder.Configuration.GetSection(nameof(AzureOpenAIOptions)))
    .ValidateDataAnnotations();

builder.Services.AddOptionsWithValidateOnStart<ResilienceOptions>()
    .Bind(builder.Configuration.GetSection(nameof(ResilienceOptions)))
    .ValidateDataAnnotations();

// Complex options with post-configuration
builder.Services.AddOptionsWithValidateOnStart<PowerBIOptions>()
                .Bind(builder.Configuration.GetSection(nameof(PowerBIOptions)))
                .Configure(options =>
                {
                    var headersJson = builder.Configuration["PowerBIOptions:Headers"];
                    if (!string.IsNullOrWhiteSpace(headersJson))
                    {
                        options.UseValues(JsonSerializer.Deserialize<Dictionary<string, string>>(headersJson)!);
                    }
                })
                .ValidateDataAnnotations();
```

#### Options Pattern Best Practices

- Always use `AddOptionsWithValidateOnStart<T>()` for fail-fast validation.
- Chain `.Bind()` and `.ValidateDataAnnotations()` for automatic binding and validation.
- Use `.Configure()` or `.PostConfigure()` for complex initialization logic.
- Throw `ArgumentException` for required connection strings.

```csharp
var tableStorageConnectionString = builder.Configuration.GetConnectionString(Constants.ConnectionStrings.TableStorage)
    ?? throw new ArgumentException($@"Missing configuration for '{Constants.ConnectionStrings.TableStorage}'!");
```

### 7. Application Services

Use structured comment `/* Application Services */` and follow this registration order.

```csharp
/* Application Services */

// HTTP and caching (Web APIs only)
builder.Services.AddHttpClient()
                .AddDistributedMemoryCache()
                .AddMemoryCache()
                .AddProblemDetails()
                .AddRouting();

// API versioning (Web APIs only)
builder.Services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
                })
                .AddMvc(options =>
                {
                    options.Conventions.Add(new VersionByNamespaceConvention());
                })
                .AddApiExplorer(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.FormatGroupName = (groupName, version) => $@"{groupName}_{version}".ToLowerInvariant();
                    options.GroupNameFormat = @"'v'V";
                });

// Controllers (Web APIs only)
builder.Services.AddControllers(options =>
                {
                    options.RequireHttpsPermanent = true;
                    options.SuppressAsyncSuffixInActionNames = true;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

// Custom services (domain-specific, all project types)
builder.Services.AddSingleton<IMyService, MyService>()
                .AddScoped<MyServiceConfiguration>()
                .AddScoped<IMyOtherService, MyOtherService>();
```

### 8. Application Middleware Configuration

Use structured comment `/* Application Middleware Configuration */`:

```csharp
/* Application Middleware Configuration */

var app = builder.Build();

app.MapDefaultEndpoints();

if (isDevelopment || isStaging)
{
    app.UseDeveloperExceptionPage();
}

if (useAppConfiguration)
{
    app.UseAzureAppConfiguration();
}

app.UseRouting()
   .UseExceptionHandler()
   .UseAuthentication()
   .UseAuthorization()
   .UseStatusCodePages()
   .UseEndpoints(endpoints =>
   {
       endpoints.MapControllers();
   });
```

### 9. Application Execution

```csharp
try
{
    await app.RunAsync();
}
finally
{
    await app.DisposeAsync();
}
```

## Project Type Templates

### Web API Template

```csharp
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Asp.Versioning.Conventions;

using Company.Project;
using Company.Project.Options;

using Azure.Identity;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var assemblyName = typeof(Program).Assembly.GetName().Name;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
});

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

/* Load Configuration */
// ... (follow pattern above)

/* Telemetry Configuration */
// ... (follow pattern above)

/* Load Options */
// ... (follow pattern above)

/* Application Services */
// ... (follow pattern above)

/* Application Middleware Configuration */
// ... (follow pattern above)

app.Run();
```

### Background Worker Template

```csharp
using System.Diagnostics;

using Company.Project;
using Company.Project.Options;

using Azure.Identity;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;

var builder = Host.CreateApplicationBuilder(args);
ConfigureBuilder(builder);

var host = builder.Build();
await host.RunAsync();

static void ConfigureBuilder(IHostApplicationBuilder builder)
{
    var assemblyName = typeof(Program).Assembly.GetName().Name;
    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

    /* Load Configuration */
    // ... (follow pattern above)

    /* Logging Configuration */
    builder.AddServiceDefaults(assemblyName ?? string.Empty);

    if (Debugger.IsAttached)
    {
        builder.Logging.AddDebug();
    }

    /* Load Options */
    // ... (follow pattern above)

    /* Application services */
    builder.Services.AddSingleton<IMyService, MyService>();
    builder.Services.AddHostedService<MyWorker>();
}
```

## Patterns to Follow

### Azure Authentication

- Always use `ChainedTokenCredential` with `DefaultAzureCredential` and `EnvironmentCredential`.
- Never hardcode credentials or connection strings.
- Store secrets in Azure Key Vault referenced through App Configuration.

### Method Chaining Style

- Use fluent method chaining with proper indentation.
- Align chained method calls vertically for readability.

```csharp
builder.Services.AddApiVersioning(options => { })
                .AddMvc(options => { })
                .AddApiExplorer(options => { });
```

### Configuration Fallback Pattern

```csharp
var defaultLocale = builder.Configuration.GetValue<string>(Constants.Settings.DefaultLocale) ?? CultureInfo.CurrentCulture.Name;
```

### Conditional Debug Configuration

```csharp
if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
    builder.Logging.AddDebug();
}
```

## Patterns to Avoid

- **Avoid hardcoded connection strings**: Use Azure App Configuration and Key Vault.
- **Avoid missing section comments**: Always use structured comments like `/* Load Configuration */`.
- **Avoid inconsistent ordering**: Follow the established section order.
- **Avoid mixing configuration patterns**: Use Options pattern consistently.
- **Avoid synchronous blocking calls**: Use async/await throughout.
- **Avoid missing telemetry**: Always configure Application Insights and OpenTelemetry.
- **Avoid missing `useAppConfiguration` checks**: Conditionally enable Azure App Configuration middleware.

## Validation

- Ensure all required configuration sections are present.
- Verify Azure App Configuration connectivity before startup.
- Use `ValidateDataAnnotations()` for options validation at startup.

## References

- [ASP.NET Core Fundamentals](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/)
- [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/)
- [Options Pattern in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/options)
- [.NET Aspire Overview](https://learn.microsoft.com/en-us/dotnet/aspire/)
