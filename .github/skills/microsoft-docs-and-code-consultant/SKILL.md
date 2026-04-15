---
name: microsoft-docs-and-code-consultant
description: "Query official Microsoft documentation to understand concepts, find tutorials, learn how services work, look up Microsoft API references, find working code samples, and verify SDK code is correct. Use when working with Azure SDKs, .NET libraries, or Microsoft APIs—to find the right method, check parameters, get working examples, or troubleshoot errors. Catches hallucinated methods, wrong signatures, and deprecated patterns by querying official docs. Get accurate, current information from `learn.microsoft.com` and other official Microsoft websites—architecture overviews, quickstarts, configuration guides, limits, and best practices."
compatibility: "Requires Microsoft Learn MCP Server (https://learn.microsoft.com/api/mcp)"
---

# Microsoft Docs

## Tools

| Tool                           | Use For                                                                             |
| ------------------------------ | ----------------------------------------------------------------------------------- |
| `microsoft_docs_search`        | Find documentation—concepts, guides, tutorials, configuration                       |
| `microsoft_docs_fetch`         | Get full page content (when search excerpts aren't enough)                          |
| `microsoft_code_sample_search` | Find official Microsoft code samples that you could use as reference or inspiration |

## When to Use

- **Understanding concepts**: "How does Cosmos DB partitioning work?"
- **Learning a service**: "Azure Functions overview", "Container Apps architecture"
- **Finding tutorials**: "quickstart", "getting started", "step-by-step"
- **Configuration options**: "App Service configuration settings"
- **Limits & quotas**: "Azure OpenAI rate limits", "Service Bus quotas"
- **Best practices**: "Azure security best practices"
- **Before writing code—find a working pattern to follow**
- **After errors—compare your code against a known-good sample**
- **Unsure of initialization/setup—samples show complete contex**

## Query Effectiveness

Good queries are specific:

```text
# BAD - Too broad
"Azure Functions"

# GOOD - Specific
"Azure Functions Python v2 programming model"
"Cosmos DB partition key design best practices"
"Container Apps scaling rules KEDA"
```

Include context:

- **Version** when relevant (`.NET 8`, `.NET 10`, `C# 14`, `EF Core 8`)
- **Task intent** (`quickstart`, `tutorial`, `overview`, `limits`)
- **Platform** for multi-platform docs (`Linux`, `Windows`)

## When to Fetch Full Page

Fetch after search when:

- **Tutorials** — need complete step-by-step instructions
- **Configuration guides** — need all options listed
- **Deep dives** — user wants comprehensive coverage
- **Search excerpt is cut off** — full context needed

## Finding Code Samples

Use `microsoft_code_sample_search` to get official, working examples:

```text
microsoft_code_sample_search(query: "upload file to blob storage", language: "csharp")
microsoft_code_sample_search(query: "authenticate with managed identity", language: "python")
microsoft_code_sample_search(query: "send message service bus", language: "javascript")
```

### API Lookups

```text
# Verify method exists (include namespace for precision)
"BlobClient UploadAsync Azure.Storage.Blobs"
"GraphServiceClient Users Microsoft.Graph"

# Find class/interface
"DefaultAzureCredential class Azure.Identity"

# Find correct package
"Azure Blob Storage NuGet package"
"azure-storage-blob pip package"
```

Fetch full page when method has multiple overloads or you need complete parameter details.

## Error Troubleshooting

- Use `microsoft_code_sample_search` to find working code samples and compare with your implementation.
- For specific errors, use `microsoft_docs_search` and `microsoft_docs_fetch`:

| Error Type         | Query                                                    |
|--------------------|----------------------------------------------------------|
| Method not found   | `"[ClassName] methods [Namespace]"`                      |
| Type not found     | `"[TypeName] NuGet package namespace"`                   |
| Wrong signature    | `"[ClassName] [MethodName] overloads"` → fetch full page |
| Deprecated warning | `"[OldType] migration v12"`                              |
| Auth failure       | `"DefaultAzureCredential troubleshooting"`               |
| 403 Forbidden      | `"[ServiceName] RBAC permissions"`                       |

## When to Verify

Always verify when:

- Method name seems "too convenient" (`UploadFile` vs actual `Upload`)
- Mixing SDK versions (v11 `CloudBlobClient` vs v12 `BlobServiceClient`)
- Package name doesn't follow conventions (`Azure.*` for .NET, `azure-*` for Python)
- Using an API for the first time

## Validation Workflow

Before generating code using Microsoft SDKs, verify it's correct:

1. **Confirm method or package exists** — `microsoft_docs_search(query: "[ClassName] [MethodName] [Namespace]")`
2. **Fetch full details** (for overloads/complex params) — `microsoft_docs_fetch(url: "...")`
3. **Find working sample** — `microsoft_code_sample_search(query: "[task]", language: "[lang]")`

For simple lookups, step 1 alone may suffice. For complex API usage, complete all three steps.

## Why Use This

- **Accuracy** — live docs, not training data that may be outdated
- **Completeness** — tutorials have all steps, not fragments
- **Authority** — official Microsoft documentation
