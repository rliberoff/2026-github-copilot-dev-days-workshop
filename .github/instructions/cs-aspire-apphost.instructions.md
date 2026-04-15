---
applyTo: "**/*.cs"
description: "Guidelines for configuring .NET Aspire AppHost projects following established patterns for service orchestration, Dapr integration, and configuration."
---

# Aspire AppHost Guidelines

Guidelines for configuring .NET Aspire AppHost projects following established architectural patterns.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Aspire Version**: Latest stable Aspire SDK
- **Service Mesh**: Dapr sidecar integration
- **Supported Resource Types**: .NET projects, Azure Functions, NPM apps, Azure services

## Critical Principles

> **CRITICAL**: The AppHost project is **orchestration code**, not application logic.

When generating or modifying AppHost code, you must:

- **Avoid introducing business logic** — No domain rules, validation, or data processing
- **Avoid cross-cutting concerns** — No authentication, authorization, retry logic, or circuit breakers (these belong in services)
- **Treat AppHost as declarative topology** — Define _what runs_ and _how services connect_, not _how they behave_
- **Preserve required ordering** — Configuration loading, Azure App Configuration, Dapr setup, resource definitions must follow canonical order
- **Never add configuration sources** unless explicitly requested by the developer
- **Fail fast on missing configuration** — Use null-forgiving operator `!` or explicit validation with meaningful exceptions
- **Prefer fewer features over more features** — When uncertain, choose the simpler approach

## AppHost Design Principles

All AppHost code must follow these principles:

1. **Code-first topology** — AppHost defines the system's shape, not its behavior
2. **No domain knowledge** — Only service names, dependencies, and infrastructure concerns
3. **Configuration-driven identifiers** — Never hardcode Application IDs, endpoints, or connection strings
4. **Explicit dependencies** — Always use `.WithReference()` or `.WaitFor()` to model service relationships
5. **Minimal imperative logic** — Prefer declarative builder patterns over conditional logic
6. **Single source of truth** — Azure App Configuration is the canonical configuration store for production

## `Program.cs` Structure (Required Order)

AI assistants **must preserve this ordering** unless explicitly instructed otherwise:

1. **Builder creation** — `DistributedApplication.CreateBuilder(args)`
2. **Configuration loading** — Base path, appsettings files, environment variables, .env files
3. **Azure App Configuration wiring** — Connect to remote configuration store
4. **Dapr configuration** — Enable Dapr runtime integration
5. **Application ID resolution** — Read all service identifiers from configuration
6. **Resource definitions** — Define services, dependencies, and infrastructure
7. **Build / Run / Dispose** — Application lifecycle management

### Canonical Implementation

```csharp
using System.Diagnostics;

using Azure.Identity;
using DotNetEnv;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

/* Load Configuration */

if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    // By design, `DefaultAzureCredential` chooses the Visual Studio 2022/2026 account (`VisualStudioCredential`) when debugging locally.
    // In some cases, the Visual Studio account may not have the expected Tenant ID.
    // If that happens, override the Tenant ID by setting the `AZURE_TENANT_ID` environment variable in a `.env` file.
    // Each developer can configure this in their own `.local.env` file (excluded from source control).
    // IMPORTANT: Only use this override if authentication fails with the default Visual Studio credentials.
    // Example: AZURE_TENANT_ID=5b0f673d-...-...-...-7d5ae537ae70
    // Reference: https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential

    Env.Load(@".env");
    Env.Load(@".local.env");
}

/* Azure App Configuration */
var appConfigurationEndpoint = builder.Configuration[Constants.AppConfig.Endpoint]!;

var useAppConfiguration = !string.IsNullOrWhiteSpace(appConfigurationEndpoint);

if (useAppConfiguration)
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(appConfigurationEndpoint), credential)
               .ConfigureKeyVault(keyVault =>
               {
                   keyVault.SetCredential(credential);
               })
               .Select(KeyFilter.Any, LabelFilter.Null) // Load configuration values with no label
               ;
    }, optional: false);

    builder.Services.AddAzureAppConfiguration();
}

/* Configure Dapr */
builder.AddDapr(options =>
{
    options.EnableTelemetry = false;
});

/* Read Application IDs */
// Application IDs must come from configuration (never hardcoded)
// Use null-forgiving operator (!) to signal required values
// Alternative: Explicit validation with meaningful exceptions
// var gatewayAppId = builder.Configuration[Constants.Settings.GatewayServiceOptions.ApplicationId]
//     ?? throw new InvalidOperationException("Gateway ApplicationId is required");

var orchestratorAppId = builder.Configuration[Constants.Settings.OrchestratorServiceOptions.ApplicationId]!;
var gatewayAppId = builder.Configuration[Constants.Settings.GatewayServiceOptions.ApplicationId]!;
var agentAAppId = builder.Configuration[Constants.Settings.AgentAServiceOptions.ApplicationId]!;
// ... more app IDs

/* Define Services */
var agentA = builder.AddProject<Projects.{ProjectName}_Services_Agents_AgentA>(agentAAppId)
                    .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                    .WithDaprSidecar();

var orchestrator = builder.AddProject<Projects.{ProjectName}_Services_Orchestrator>(orchestratorAppId)
                          .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                          .WithReference(agentA)
                          .WithDaprSidecar();

var gateway = builder.AddProject<Projects.{ProjectName}_Services_Gateway>(gatewayAppId)
                     .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                     .WithReference(orchestrator)
                     .WithDaprSidecar(new DaprSidecarOptions()
                     {
                         AppId = gatewayAppId,
                         DaprMaxBodySize = @"200Mi",
                     });

/* Build and Run */
var app = builder.Build();

try
{
    await app.RunAsync();
}
finally
{
    await app.DisposeAsync();
}
```

## Resource Definition Patterns

### Basic Service with Dapr

```csharp
var service = builder.AddProject<Projects.{ProjectName}_Services_{ServiceName}>(appId)
                     .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                     .WithDaprSidecar();
```

### Service with Custom Dapr Options

```csharp
var service = builder.AddProject<Projects.{ProjectName}_Services_Gateway>(gatewayAppId)
                     .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                     .WithDaprSidecar(new DaprSidecarOptions()
                     {
                         AppId = gatewayAppId,
                         DaprMaxBodySize = @"200Mi",            // For large file uploads
                         ResourcesPaths = [daprComponentsPath], // Custom Dapr components
                     });
```

### Service with Dependencies

```csharp
// Define agents/services first
var agentA = builder.AddProject<Projects.{ProjectName}_Services_Agents_AgentA>(agentAAppId)
                    .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                    .WithDaprSidecar();

var agentB = builder.AddProject<Projects.{ProjectName}_Services_Agents_AgentB>(agentBAppId)
                    .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                    .WithDaprSidecar();

// Orchestrator depends on agents
var orchestrator = builder.AddProject<Projects.{ProjectName}_Services_Orchestrator>(orchestratorAppId)
                          .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                          .WithReference(agentA)  // Dependency
                          .WithReference(agentB)  // Dependency
                          .WithDaprSidecar();

// Gateway depends on orchestrator
var gateway = builder.AddProject<Projects.{ProjectName}_Services_Gateway>(gatewayAppId)
                     .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                     .WithReference(orchestrator)
                     .WithDaprSidecar();
```

### Azure Functions Integration

**Mandatory Rules:**

- Storage is **required** for all Azure Functions in Aspire
- Storage emulator is **preferred** for local development
- Functions must be referenced explicitly in dependent services
- Use `.WithExternalHttpEndpoints()` to expose HTTP triggers

```csharp
// Add storage emulator
var storage = builder.AddAzureStorage(@"storage")
                     .RunAsEmulator();

// Add Azure Functions project
var queryExecution = builder.AddAzureFunctionsProject<Projects.{ProjectName}_QueryExecutionFunction>(appId)
                            .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                            .WithExternalHttpEndpoints()
                            .WithHostStorage(storage) // Required by .NET Aspire design
                            .WithDaprSidecar();

// Reference the function in other services
var agentQueries = builder.AddProject<Projects.{ProjectName}_Services_Agents_Queries>(agentQueriesAppId)
                          .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                          .WithReference(queryExecution) // Service can now invoke the function
                          .WithDaprSidecar();
```

**Key Points:**

- Azure Functions in Aspire **require** storage via `.WithHostStorage(storage)`
- Use `.WithExternalHttpEndpoints()` to expose function HTTP triggers
- Storage emulator runs automatically for local development
- Reference: [.NET Aspire Azure Functions Integration](https://learn.microsoft.com/en-us/dotnet/aspire/serverless/functions)

### NPM Applications Integration

**Mandatory Rules:**

- Treat NPM apps as **external processes** with minimal coupling
- **Only use environment variables** for integration (no direct code references)
- NPM apps should **not have Dapr sidecars** (they are frontends, not backend services)
- Always use `.WaitFor()` to ensure backend readiness before starting frontend

**Integration Pattern:**

```csharp
// Frontend application (React, Vue, Angular, etc.)
var webInsights = builder.AddNpmApp(appIdWebInsights, @"../ActioGlobal.AI.Services.Insights", @"dev")
                         .WithEnvironment(@"VITE_API_BASE", apiInsights.GetEndpoint(@"http"))
                         .WithEnvironment(@"VITE_API_VERSION", @"v1")
                         .WithEnvironment(@"VITE_APP_INSIGHTS_INSTRUMENTATION_KEY", appInsightsKey)
                         .WithEnvironment(@"BROWSER", @"none") // Prevent auto-opening browser
                         .WithNpmPackageInstallation() // Automatically runs npm install
                         .WaitFor(apiInsights) // Wait for backend to be ready
                         .WithHttpEndpoint(8081, env: "VITE_PORT")
                         .WithExternalHttpEndpoints()
                         ;

// Web Chat frontend with Bot Framework Direct Line
var botFrontend = builder.AddNpmApp(appIdBotFrontend, @"../ActioGlobal.AI.Services.Bot.WebChat", @"start")
                         .WithEnvironment(@"WEB_CHAT_DIRECT_LINE_DOMAIN", directLineEndpoint)
                         .WithEnvironment(@"WEB_CHAT_DIRECT_LINE_TOKEN", directLineToken)
                         .WithEnvironment(@"WEB_CHAT_ASSETS_BASE_URL", @"/assets/")
                         .WithEnvironment(@"WEB_CHAT_PREFIX", @"myproject-bot-")
                         .WaitFor(botBackend)
                         .WithHttpEndpoint(8080, env: @"PORT")
                         .WithExternalHttpEndpoints();
```

**Key Methods:**

- `.AddNpmApp(name, workingDirectory, scriptName)` - Add NPM application
- `.WithNpmPackageInstallation()` - Automatically run `npm install` before start
- `.WaitFor(resource)` - Wait for dependent service to be ready
- `.WithHttpEndpoint(port, env: "ENV_VAR")` - Expose HTTP endpoint with environment variable
- `.WithExternalHttpEndpoints()` - Make service accessible from outside the app
- `.GetEndpoint("http")` - Get the endpoint URL of a service for configuration

**Common Use Cases:**

| Frontend Framework | Start Script | Port Env Var | Notes                          |
| ------------------ | ------------ | ------------ | ------------------------------ |
| Vite (React/Vue)   | `dev`        | `VITE_PORT`  | Use `VITE_*` for env variables |
| Create React App   | `start`      | `PORT`       | Standard CRA configuration     |
| Next.js            | `dev`        | `PORT`       | Server-side rendering support  |
| Angular            | `start`      | `PORT`       | Use `NG_APP_*` for env vars    |
| Nuxt.js            | `dev`        | `PORT`       | Vue-based SSR framework        |

### Service Startup Dependencies

Use `.WaitFor()` to control service startup order:

```csharp
// Backend services start first
var apiService = builder.AddProject<Projects.MyProject_Api>(apiAppId)
                        .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                        .WithDaprSidecar();

// Frontend waits for backend to be ready
var frontend = builder.AddNpmApp(frontendAppId, @"../MyProject.Frontend", @"start")
                      .WaitFor(apiService) // Won't start until API is ready
                      .WithEnvironment(@"API_URL", apiService.GetEndpoint(@"http"))
                      .WithHttpEndpoint(3000, env: @"PORT")
                      .WithExternalHttpEndpoints();
```

## Constants Pattern

**Rules:**

- Constants in AppHost **may use `const` safely** (no cross-assembly consumption risk)
- All constants must remain `internal` (AppHost is not a shared library)
- Use `@` verbatim strings for configuration keys to avoid escaping issues

```csharp
namespace {ProjectName}.Aspire.AppHost;

internal static class Constants
{
    internal static class AppConfig
    {
        internal const string Endpoint = @"AppConfig:Endpoint";
    }

    internal static class Settings
    {
        internal static class AssistantServiceOptions
        {
            internal const string ApplicationId = @"AssistantServiceOptions:ApplicationId";
        }

        internal static class BotServiceOptions
        {
            internal const string ApplicationId = @"BotServiceOptions:ApplicationId";
        }

        internal static class BotFrontendServiceOptions
        {
            internal const string ApplicationId = @"BotFrontendServiceOptions:ApplicationId";
        }

        internal static class DirectLineOptions
        {
            internal const string Endpoint = @"DirectLineOptions:Endpoint";
            internal const string Token = @"DirectLineOptions:Token";
        }

        internal static class AppInsightsOptions
        {
            internal const string InstrumentationKey = @"AppInsightsOptions:InstrumentationKey";
        }

        // Add more service options as needed
    }
}
```

## Configuration Hierarchy

Configuration is loaded in this order (later overrides earlier):

1. `appsettings.json` - Base configuration
2. `appsettings.debug.json` - Debugger-attached only (use `if (Debugger.IsAttached)`)
3. `appsettings.{Environment}.json` - Environment-specific (Development, Staging, Production)
4. `appsettings.{Username}.json` - Developer-specific (e.g., `appsettings.john.json`)
5. Environment variables
6. `.env` and `.local.env` files - Development only (requires `DotNetEnv` package)
7. Azure App Configuration (with Key Vault references) - Production ready

### `.env` File Usage

- **`.env`**: Shared development settings, committed to source control
- **`.local.env`**: Developer-specific overrides, **excluded from source control** (add to `.gitignore`)
- **Use case**: Override `AZURE_TENANT_ID` when Visual Studio credentials use a different tenant
- **Pattern**: Only load in development environment using `builder.Environment.IsDevelopment()`

```csharp
if (builder.Environment.IsDevelopment())
{
    Env.Load(@".env");
    Env.Load(@".local.env");
}
```

### Azure App Configuration

#### Identity-Based Authentication (Preferred)

- **Always prefer** `DefaultAzureCredential` for production deployments.
- Connection strings are allowed **only when explicitly requested** by the developer.
- Key Vault integration is **mandatory** when using App Configuration.
- Use null-forgiving operator `!` to signal required configuration values.

#### Endpoint with DefaultAzureCredential

```csharp
var appConfigurationEndpoint = builder.Configuration[Constants.AppConfig.Endpoint]!;
var credential = new DefaultAzureCredential();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfigurationEndpoint), credential)
           .ConfigureKeyVault(keyVault => keyVault.SetCredential(credential))
           .Select(KeyFilter.Any, LabelFilter.Null);
}, optional: false);
```

#### Connection String (Secret-based)

```csharp
var appConfigurationConnectionString = builder.Configuration.GetConnectionString(@"AppConfig");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(appConfigurationConnectionString)
           .ConfigureKeyVault(keyVault => keyVault.SetCredential(new DefaultAzureCredential()))
           .Select(KeyFilter.Any, LabelFilter.Null);
}, optional: false);
```

## appsettings.json Structure

```json
{
  "AppConfig": {
    "Endpoint": "https://myconfig.azconfig.io"
  },
  "OrchestratorServiceOptions": {
    "ApplicationId": "myproject-services-orchestrator"
  },
  "GatewayServiceOptions": {
    "ApplicationId": "myproject-services-gateway"
  },
  "AgentAServiceOptions": {
    "ApplicationId": "myproject-services-agents-agenta"
  },
  "DirectLineOptions": {
    "Endpoint": "https://directline.botframework.com/v3/directline",
    "Token": "your-directline-token"
  },
  "AppInsightsOptions": {
    "InstrumentationKey": "your-instrumentation-key"
  }
}
```

## Dapr Integration (Conceptual Rules)

> **Key Concept**: Aspire hosts Dapr. Dapr defines distributed behavior.

AI assistants must:

- **Never reimplement Dapr primitives** — Use Dapr's built-in state management, pub/sub, service invocation
- **Never bypass Dapr for service communication** — All service-to-service calls should use Dapr service invocation
- **Enable Dapr sidecars for all backend services** — Frontend/NPM apps are exceptions
- **Dapr telemetry is handled by Aspire** — Set `EnableTelemetry = false` in Dapr options

### Canonical Dapr Setup

```csharp
builder.AddDapr(options =>
{
    options.EnableTelemetry = false;
});
```

### Custom Dapr Options (When Needed)

```csharp
.WithDaprSidecar(new DaprSidecarOptions()
{
    AppId = gatewayAppId,
    DaprMaxBodySize = @"200Mi",            // ONly when large file uploads and downloads are required or needed
    ResourcesPaths = [daprComponentsPath], // Custom component definitions
});
```

## Dapr Components

Place Dapr component definitions in `components/` folder:

## Service Architecture

### Hierarchical Dependency Model

```text
Gateway Service (Entry Point)
    └── Orchestrator Service
            ├── Agent A
            ├── Agent B
            └── Agent C

Independent Services:
├── Worker Service
└── Background Jobs
```

### Service Communication

- **Dapr Service Invocation**: Services communicate via Dapr sidecars
- **HTTP with Service Discovery**: Use `WithReference()` for automatic endpoint resolution
- **Application IDs**: Read from configuration, not hardcoded

### Dependency Modeling (Strict Rules)

> **Rule**: Dependencies must reflect **runtime invocation paths only**.

**Use `.WithReference()` for:**

- Direct service-to-service HTTP calls
- Dapr service invocation dependencies
- Services that must start in a specific order due to initialization requirements

**Never add references for:**

- Shared logging infrastructure (handled by service defaults)
- Configuration sources (each service loads independently)
- Shared libraries or NuGet packages (compile-time only)
- Databases or message queues (use connection strings instead)

```csharp
// GOOD - Orchestrator invokes agents at runtime → use WithReference
var orchestrator = builder.AddProject<Projects.MyProject_Services_Orchestrator>(orchestratorAppId)
                          .WithReference(agentA)
                          .WithReference(agentB)
                          .WithDaprSidecar();

// BAD - Don't add references just because services share a database
var serviceA = builder.AddProject<Projects.MyProject_ServiceA>(appIdA)
                      .WithReference(database); // WRONG - use connection string instead
```

## Naming Conventions

| Element           | Convention              | Example                                     |
| ----------------- | ----------------------- | ------------------------------------------- |
| Application ID    | `kebab-case`            | `myproject-services-agents-agenta`          |
| Project Reference | Underscores             | `Projects.MyProject_Services_Agents_AgentA` |
| Configuration Key | `PascalCase:PascalCase` | `AssistantServiceOptions:ApplicationId`     |
| Dapr Component    | `kebab-case`            | `azure-queue-messages`                      |

## Service Defaults

All services use shared service defaults for telemetry and health:

```csharp
// In each service's Program.cs
builder.AddServiceDefaults(assemblyName);
```

This provides:

- OpenTelemetry configuration (traces, metrics, logs)
- Health checks (`/alive`, `/health`)
- Service discovery
- HTTP client resilience (retry, timeout, circuit breaker)
- Azure Monitor integration

## Environment-Specific Overrides

### Development

- Debug configuration loaded
- Swagger UI enabled
- Detailed error messages

### Production

- Azure App Configuration as source of truth
- Minimized logging verbosity
- Security middleware enabled

## Adding New Services

1. Create the service project in `src/`
2. Add project reference to AppHost `.csproj`
3. Add configuration key in `Constants.cs`
4. Add app ID to configuration files
5. Register in `Program.cs` with appropriate dependencies

```csharp
// Add to Constants.cs
internal static class NewServiceOptions
{
    internal const string ApplicationId = @"NewServiceOptions:ApplicationId";
}

// Add to Program.cs
var newServiceAppId = builder.Configuration[Constants.Settings.NewServiceOptions.ApplicationId]!;

var newService = builder.AddProject<Projects.{ProjectName}_Services_NewService>(newServiceAppId)
                        .WithEnvironment(Constants.AppConfig.Endpoint, appConfigurationEndpoint)
                        .WithDaprSidecar();

// If other services depend on it, add .WithReference(newService)
```

## Error Handling and Disposal

Always use proper error handling and disposal pattern:

- **try-finally** (no catch): Let exceptions propagate naturally — Aspire and the host handle logging
- **finally**: Ensures proper cleanup of resources (Dapr sidecars, containers, etc.) even when exceptions occur
- **DisposeAsync**: Gracefully shuts down all managed resources and services
- Do not add global exception handling in AppHost. Let the hosting infrastructure manage error reporting.

```csharp
var app = builder.Build();

try
{
    await app.RunAsync();
}
finally
{
    await app.DisposeAsync();
}
```

## Anti-Patterns (Explicitly Forbidden)

**Never** generate code containing these patterns:

### Business Logic in AppHost

```csharp
// BAD - Business logic does not belong here
if (customerType == @"Premium")
{
    builder.AddProject<Projects.PremiumService>(appId);
}
```

### Conditional Service Wiring Based on Runtime Data

```csharp
// BAD - Service topology must be static
var currentHour = DateTime.Now.Hour;
if (currentHour > 18)
{
    builder.AddProject<Projects.NightService>(appId);
}
```

### Direct HTTP Calls Between Services (Bypassing Dapr)

```csharp
// BAD - Use Dapr service invocation instead
using var httpClient = new HttpClient();
var result = await httpClient.GetAsync(@"http://localhost:5001/api/data");
```

### Singleton State in AppHost

```csharp
// BAD - AppHost should not maintain state
public static class ServiceCache
{
    public static Dictionary<string, string> Data { get; } = new();
}
```

### Hardcoded Configuration Values

```csharp
// BAD - Always use configuration
var service = builder.AddProject<Projects.MyService>(@"hardcoded-app-id");
```

### Multiple AppHost Projects Per Solution

```text
WRONG - One solution should have exactly one AppHost
src/
├── AppHost1/
├── AppHost2/
└── Services/
```

**Correct**: One AppHost orchestrates all services in the solution.

## Required NuGet Packages

Ensure these packages are referenced in your AppHost project:

```xml
<PackageReference Include="Aspire.Hosting.AppHost" />
<PackageReference Include="Aspire.Hosting.Azure.Storage" /> <!-- For Azure Functions storage -->
<PackageReference Include="Azure.Identity" />
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Dapr" /> <!-- For Dapr integration -->
<PackageReference Include="CommunityToolkit.Aspire.Hosting.NodeJs" /> <!-- For NPM apps -->
<PackageReference Include="DotNetEnv" /> <!-- For .env file support (development only) -->
<PackageReference Include="Microsoft.Extensions.Configuration.AzureAppConfiguration" />
```

## Final Architectural Guidance

### Intended Outcomes

Following these guidelines ensures:

- **Predictable code** — Consistent patterns reduce unexpected behavior
- **Correct Aspire + Dapr usage** — Leverage platform capabilities properly
- **Consistent environments** — Local development matches production topology
- **Long-term maintainability** — Clear separation of concerns prevents architectural decay
- **Fail-fast behavior** — Missing configuration is caught at startup, not runtime

### Guiding Principle

When uncertain about adding code to AppHost, ask:

1. **Does this define what services run?** → Belongs in AppHost
2. **Does this define how services are connected?** → Belongs in AppHost
3. **Does this define how a service behaves?** → Belongs in the service itself, not to AppHost
4. **Would this appear on an architecture diagram?** → Probably belongs in AppHost
5. **Is this business logic or domain knowledge?** → Never belongs in AppHost

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
