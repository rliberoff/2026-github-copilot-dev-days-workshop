---
applyTo: "**/*.cs"
description: "Coding style conventions for C# and .NET projects. When generating C# code, always follow these guidelines."
---

# C# Coding Style Guidelines

Guidelines for writing clean, maintainable, and consistent C# code following modern .NET conventions.

## Project Context

- **Target Framework**: .NET 10 with C# 14
- **Style Reference**: [Microsoft .NET C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- **Formatting**: `.editorconfig` file takes precedence when present

## General Principles

- **Clarity over brevity**: Prefer readable code over clever shortcuts.
- **Consistency**: Use uniform formatting and naming throughout the codebase.
- **Documentation**: Write clear and concise comments for public APIs.
- **Modern features**: Leverage C# 14 language features appropriately.

## Naming Conventions

- Use `PascalCase` for class, method, and property names.
- Use `camelCase` for local variables and method parameters.
- Use `ALL_CAPS` for constants.
- Prefix interfaces with `I` (e.g., `IOrderService`).
- Use meaningful, descriptive names; avoid abbreviations.
- Never prefix private fields with underscores (`_`). Use `camelCase` instead.

## Formatting

- **Always** apply code-formatting style defined in `.editorconfig`.
- Use 4 spaces for indentation (no tabs).
- Use file-scoped namespaces to simplify structure and improve readability.
- Add a blank line between method definitions.
- Insert a newline before the opening curly brace of any code block (e.g., after `if`, `for`, `while`, `foreach`, `using`, `try`, etc.).
- Place opening braces on a new line for methods, properties, and types (unless using file-scoped namespaces, then follow the file-scoped style).
- Use pattern matching and switch expressions wherever possible.
- Use `nameof` instead of string literals when referring to member names.
- Use `string.Empty` instead of `""` for empty strings.
- Ensure that XML doc comments are created for any public APIs. When applicable, include `<example>` and `<code>` documentation in the comments.

### Example: File-Scoped Namespaces

```csharp
// BAD - Before
namespace MyNamespace
{
    public class ExampleClass
    {
        // ...existing code...
    }
}

// GOOD - After
namespace MyNamespace;

public class ExampleClass
{
    // ...existing code...
}
```

- All new files must use file-scoped namespaces. Refactor existing files during updates or maintenance.

## Variable Declaration

- Use `var` for local variable declarations when the type is obvious.
- Prefer explicit types if it improves clarity.

### Variable Declaration Example

```csharp
// BAD - Before
int x = 1;
double y = 2.0;
string z = "Hello";
ProductBacklogItem item = new ProductBacklogItem("Test", "Test", 1, 1, 1);

// GOOD - After
var x = 1;
var y = 2.0;
var z = "Hello";
var item = new ProductBacklogItem("Test", "Test", 1, 1, 1);
```

## Sealed Classes

- Make classes `sealed` by default. If a class needs to be inherited, mark it as `virtual` explicitly.

## Use Nameof with Exceptions

- When throwing exceptions, use `nameof` to refer to the parameter name instead of hardcoding it.

### Nameof Example

```csharp
// BAD - Before
throw new ArgumentNullException("parameterName");

// GOOD - After
throw new ArgumentNullException(nameof(parameterName));
```

## Code Structure

- **Always** use one type per file (class, interface, enum, etc.).
- Organize files by feature/domain when possible.
- Group `using` directives at the top of the file, outside the namespace, with blank lines between groups.
- All `System` namespaces should be listed first, and the rest should be in alphabetical order.
- Place related types in the same namespace.
- Use partial classes only when necessary (e.g., for code generation).
- Prefer one-liner sentences for methods, properties, and constructors rather than spanning multiple lines.

## Comments & Documentation

- Use XML documentation comments (`///`) for public members and types.
- Write comments to explain why, not what, when necessary.
- Remove commented-out code before committing.
- All comments and documentation must be always in English. Specific terms can be in the original language if they are widely recognized, and between backticks (e.g., `Alvarán`).

## Null Checks & Exceptions

- Use guard clauses for argument validation.
- Use `nameof` for parameter names in exceptions.
- Use the latest version of the `CommunityToolkit.Diagnostics` package for guard clauses when available.

## Modern C# Features

- Use pattern matching and expression-bodied members where appropriate.
- Prefer object and collection initializers.
- Avoid using primary constructors.
- Prefer asynchronous methods (`async`/`await`) with cancellation tokens.
- Inject dependencies through built-in Dependency Injector (DI) container.
- Use trace capabilities and logs via `ILogger<T>`.

## Configuration Files

If applicable for the type of project (for example web application), manage different settings files for different environments and users as follows:

```csharp
if (Debugger.IsAttached)
{
    builder.Configuration.AddJsonFile(@"appsettings.debug.json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddJsonFile($@"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddJsonFile($@"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();
```

## Nullable Reference Types

- Declare variables non-nullable, and check for `null` at entry points.
- Always use `is null` or `is not null` instead of `== null` or `!= null`.
- Trust the C# null annotations and don't add null checks when the type system says a value cannot be null.

## References

- [Microsoft .NET C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
