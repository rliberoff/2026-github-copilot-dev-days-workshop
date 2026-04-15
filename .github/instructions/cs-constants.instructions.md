---
applyTo: "**/*.cs,**/Constants.cs"
description: "Guidelines for organizing constants and magic strings following established nested static class patterns."
---

# Constants Organization Guidelines

Instructions for organizing constants and magic strings following established architectural patterns.

## Project Context

- **Purpose**: Centralize magic strings, configuration keys, and identifiers
- **Pattern**: Nested static classes for hierarchical organization
- **Scope**: Project-specific constants, with shared constants in Commons

## Basic Structure

```csharp
namespace {ProjectName}.Services.{ServiceName};

internal static class Constants
{
    internal static class ConnectionStrings
    {
        internal const string TableStorage = @"TableStorage";
        internal const string CosmosDb = @"CosmosDb";
        internal const string AppConfig = @"AppConfig";
    }

    internal static class Settings
    {
        internal const string DefaultLocale = @"DefaultLocale";
        internal const string MaxRetries = @"MaxRetries";
    }

    internal static class TelemetryEvents
    {
        internal const string QueryExecuted = @"QueryExecuted";
        internal const string AgentInvoked = @"AgentInvoked";
    }
}
```

## Common Categories

### Connection Strings

```csharp
internal static class ConnectionStrings
{
    internal const string TableStorage = @"TableStorage";
    internal const string TableStorageResponses = @"TableStorageResponses";
    internal const string CosmosDb = @"CosmosDb";
    internal const string AppConfig = @"AppConfig";
    internal const string ApplicationInsights = @"ApplicationInsights";
}
```

### Configuration Settings

```csharp
internal static class Settings
{
    internal const string DefaultLocale = @"DefaultLocale";
    internal const string ApplicationId = @"ApplicationId";
    internal const string Sentinel = @"Sentinel";
}
```

### Telemetry Events

```csharp
internal static class TelemetryEvents
{
    internal const string RequestStarted = @"{ProjectName}RequestStarted";
    internal const string RequestCompleted = @"{ProjectName}RequestCompleted";
    internal const string RequestFailed = @"{ProjectName}RequestFailed";
    internal const string QueryExecuted = @"QueryExecuted";
    internal const string CacheHit = @"CacheHit";
    internal const string CacheMiss = @"CacheMiss";
}
```

### HTTP Headers

```csharp
internal static class HttpHeaders
{
    internal const string ActivityId = @"X-Activity-Id";
    internal const string UserId = @"X-User-Id";
    internal const string CorrelationId = @"X-Correlation-Id";
    internal const string Locale = @"X-Locale";
}
```

### Table Names

```csharp
internal static class TableNames
{
    internal const string IntentCategories = @"IntentCategories";
    internal const string UserProfiles = @"UserProfiles";
    internal const string Responses = @"Responses";
}
```

## Bot-Specific Constants

```csharp
internal static class Constants
{
    internal static class AdaptiveCardsActions
    {
        internal const string ActionIdentifier = @"action";
        internal const string SubmitFeedback = @"submitFeedback";
        internal const string SubmitContinueResponse = @"submitContinueResponse";
    }

    internal static class BotEvents
    {
        internal const string Welcome = @"webchat/welcome";
        internal const string StartConversation = @"startConversation";
    }

    internal static class BotInvokeActions
    {
        internal const string SubmitAction = @"message/submitAction";
    }

    internal static class BotIntents
    {
        internal const string AssistantError = @"AssistantError";
        internal const string UnauthorizedAccess = @"UnauthorizedAccess";
        internal const string Welcome = @"Welcome";
        internal const string Goodbye = @"Goodbye";
    }
}
```

## Agent-Specific Constants

```csharp
internal static class Constants
{
    internal static class Plugins
    {
        internal const string PluginDirectory = @"Plugins";

        internal static class DaxPlugin
        {
            internal const string Name = nameof(DaxPlugin);
        }
    }

    internal static class Parameters
    {
        internal const string Input = @"input";
        internal const string Context = @"context";
        internal const string Locale = @"locale";
    }

    internal static class AgentNames
    {
        internal const string DaxHelper = @"DaxHelper";
        internal const string PerformanceAnalyzer = @"PerformanceAnalyzer";
    }
}
```

## AppHost Constants

```csharp
internal static class Constants
{
    internal static class ConnectionStrings
    {
        internal const string AppConfig = @"ConnectionStrings:AppConfig";
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

        internal static class DaxHelperServiceOptions
        {
            internal const string ApplicationId = @"DaxHelperServiceOptions:ApplicationId";
        }
    }
}
```

## Shared Constants (Commons)

For constants used across multiple projects, place in a shared commons library:

```csharp
namespace {ProjectName}.Services.Commons;

public static class CommonConstants
{
    public static class EndpointGroupName
    {
        public const string AI = @"AI";
        public const string Health = @"Health";
        public const string Admin = @"Admin";
    }

    public static class Agents
    {
        public const string ActionName = @"Asks";
    }

    public static class HttpHeaders
    {
        public const string ActivityId = @"x-activity-id";
        public const string UserId = @"x-user-id";
        public const string CorrelationId = @"x-correlation-id";
        public const string Locale = @"x-locale";
        public const string ApiKey = @"x-api-key";
    }

    public static class ContentTypes
    {
        public const string Json = @"application/json";
        public const string AdaptiveCard = @"application/vnd.microsoft.card.adaptive";
    }
}
```

## Naming Conventions

| Element      | Convention              | Example                            |
| ------------ | ----------------------- | ---------------------------------- |
| Class        | `internal static class` | `internal static class Constants`  |
| Nested class | `internal static class` | `internal static class Settings`   |
| Constant     | `internal const string` | `internal const string MaxRetries` |
| Value        | Verbatim string `@""`   | `@"TableStorage"`                  |

## Usage Patterns

### Configuration Keys

```csharp
// Reading configuration
var connectionString = builder.Configuration.GetConnectionString(Constants.ConnectionStrings.TableStorage);
var appId = builder.Configuration[Constants.Settings.ApplicationId];
```

### Tracking Telemetry Events

```csharp
// Tracking events
telemetryClient.TrackEvent(Constants.TelemetryEvents.QueryExecuted, new Dictionary<string, string>
{
    [@"QueryType"] = queryType,
    [@"Duration"] = duration.ToString(),
});
```

### Adding HTTP Headers

```csharp
// Adding headers
httpRequest.Headers.Add(Constants.HttpHeaders.ActivityId, activityId);
httpRequest.Headers.Add(Constants.HttpHeaders.UserId, userId);
```

## File Location

- Project-specific: `{Project}/Constants.cs`
- Shared: `{ProjectName}.Commons/CommonConstants.cs`
- Settings defaults: `{Project}/SettingsConstants.cs` (if separate from Constants)

## Best Practices

1. **Use verbatim strings**: Always use `@""` for constant values to avoid escape issues
2. **Hierarchical organization**: Group related constants in nested classes
3. **Descriptive names**: Use clear, self-documenting names
4. **Internal visibility**: Use `internal` for project-specific constants
5. **Public for shared**: Use `public` only for constants in Commons
6. **Avoid duplication**: Reference `Commons` constants instead of duplicating
7. **Document purpose**: Add XML comments for non-obvious constants

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
