---
applyTo: "**/*Agent*/**/*.cs"
description: "Guidelines and coding patterns for creating C# AI Agent projects using Microsoft Agent Framework (MAF)"
---

# C# AI Agents Guidelines

Guidelines for creating AI Agent projects using the Microsoft Agent Framework (MAF) for .NET. These agents use LLMs to autonomously react to events, process inputs, make decisions, call tools, and generate responses.

## Project Context

- **Target Framework**: .NET 10.0 and C# 14
- **Architecture**: ASP.NET Core Web API exposing AI agents as HTTP endpoints

## When to Use AI Agents

**Use for:**

- Autonomous decision-making with ad hoc planning
- Multi-turn conversations with memory
- Unstructured tasks requiring exploration
- Integration with external tools via function calling

**Don't use for:**

- Structured tasks with predefined rules (use regular code)
- Strict workflows (use MAF Workflows instead)
- Agents needing 20+ tools (use multi-agent orchestration)

## Project Structure

```text
{CompanyName}.AI.Services.Agents.{AgentName}/
├── Controllers/V1/AIController.cs             # API endpoint
├── Models/{ModelEntity}.cs                    # Entities, DTOs or objects used by the agent
├── Tools/{ToolName}Tool.cs                    # Agent tools and functions
├── Options/{AgentName}Options.cs              # Configuration with validation
├── Services/                                  # Business logic used by the agent from the controller
├── Constants.cs
└── Program.cs                                 # Startup and DI
```

**Naming Conventions:**

- Project: `{CompanyName}.AI.Services.Agents.{AgentName}`
- Controller: `AIController` in `Controllers/V1/`
- Action: `AnswerAsync` or `{Verb}Async`
- Tools: `{ToolName}Tool` (e.g., `EntraIdUserSearchTool`)
- Services record: `{AgentName}Services`

## Program.cs Configuration

### Essential Services

```csharp
// Telemetry - OpenTelemetry + Application Insights
builder.AddServiceDefaults(assemblyName);
builder.AddAIRequestActivityProcessor();
builder.Services.AddVersionTelemetry();
builder.Services.AddAssemblyNameTelemetry(assemblyName ?? string.Empty);
builder.Services.AddAIRequestScopeTelemetry();
builder.Services.AddApplicationInsightsTelemetryClientOnly(builder.Configuration);

// API Configuration
builder.Services.AddHealthChecks();
builder.Services
    .AddProblemDetails()
    .AddMemoryCache()
    .AddDistributedMemoryCache()
    .AddApiVersioning(options => {
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
    .AddMvc(options => options.Conventions.Add(new VersionByNamespaceConvention()))
    .AddControllers(options => {
        options.Filters.Add<AgentCustomEventsFilter>();
    })
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Swagger
builder.Services.AddSwaggerGen(options => {
    options.CustomOperationIds(apiDescription =>
        apiDescription.ActionDescriptor is ControllerActionDescriptor cad
            ? cad.ActionName
            : apiDescription.TryGetMethodInfo(out var mi)
                ? mi.Name.RemoveAsyncSuffix()
                : string.Empty);
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{assemblyName}.xml"));
    options.EnableAnnotations();
});

// AI Chat Client
builder.Services.AddAzureOpenAIChatClient(builder.Configuration);

// Cosmos DB Message Store
var containerId = builder.Configuration["{AgentName}Options:CosmosDBContainerName"];
builder.Services.AddCosmosChatMessageStoreFactory(containerId);

// Services Record
builder.Services.AddScoped<{AgentName}AIControllerServices>();
```

## Controller Implementation

```csharp
[ApiController]
[EndpointGroupName(CommonConstants.EndpointGroupName.AI)]
[Route(@"api/v1/[controller]/[action]")]
public sealed class AIController : ControllerBase
{
    private readonly {AgentName}AIControllerServices services;

    public AIController({AgentName}AIControllerServices services) => this.services = services;

    [HttpPost]
    [ActionName(CommonConstants.Agents.ActionName)]
    [SwaggerOperation(Summary = "...", Description = "...")]
    [SwaggerResponse(StatusCodes.Status200OK, typeof(Answer))]
    public async Task<IActionResult> AnswerAsync([FromBody] AgentRequest request, CancellationToken cancellationToken)
    {
        services.Logger.LogInformation(@"{Method} called. Input: {Input}", nameof(AnswerAsync), request.Input);

        // Validate required headers
        if (!HttpContext.TryGetRequestHeaderValue(CommonConstants.HttpHeaders.ConversationId, out var conversationId))
        {
            return BadRequest($@"Missing required header: {CommonConstants.HttpHeaders.ConversationId}");
        }

        try
        {
            // Agent implementation
            var result = await ExecuteAgentAsync(request, conversationId, cancellationToken);
            return Ok(result);
        }
        catch (Exception exception)
        {
            services.Logger.LogError(exception, @"{Method} error! Request: {Input}", nameof(AnswerAsync), request.Input);
            throw;
        }
    }
}
```

## Agent Creation Patterns

### Pattern 1: Basic Agent with Tools

```csharp
// Initialize Tools
var tool1 = new Tool1(dependencies);
var tool2 = new Tool2(dependencies);

// Create AIFunctions from tool methods
var function1 = AIFunctionFactory.Create(tool1.MethodAsync);
var function2 = AIFunctionFactory.Create(tool2.MethodAsync);

// Define agent instructions
var instructions = """
    You are an AI Agent that {describes agent purpose}.

    You MUST:
      1) {Step 1 instruction}
      2) {Step 2 instruction}
      3) Always respond in the user's language

    {Additional guidelines}
    """;

// Get or create message store for conversation persistence
var messageStore = await services.MessageStoreFactory(conversationId);

// Configure agent options
var agentOptions = new ChatClientAgentOptions
{
    Name = @"{AgentName}",
    Description = @"{Agent description}",
    ChatMessageStoreFactory = _ => messageStore,
    ChatOptions = new ChatOptions
    {
        Tools = [function1, function2],
        Instructions = instructions,
        Temperature = 1.0f,
    },
};

// Create and build agent with telemetry
AIAgent agent = services.ChatClient.CreateAIAgent(agentOptions)
    .AsBuilder()
    .UseOpenTelemetry(
        sourceName: "*Microsoft.Agents.AI",
        configure: cfg => cfg.EnableSensitiveData = true)
    .Build();

// Create thread for this conversation
var thread = agent.GetNewThread();

// Run agent
var agentResponse = await agent.RunAsync(request.Input ?? string.Empty, thread, cancellationToken: cancellationToken);

return agentResponse.ToString();
```

### Pattern 2: Agent with Custom OpenAI Client

```csharp
// Create OpenAI client with custom endpoint
var client = new OpenAIClient(
    new ApiKeyCredential(services.Options.CurrentValue.Token),
    new OpenAIClientOptions
    {
        Endpoint = new Uri(services.Options.CurrentValue.BaseUrl),
    });

var messageStore = await services.MessageStoreFactory(conversationId);

var agentOptions = new ChatClientAgentOptions
{
    Name = deploymentName,
    ChatMessageStoreFactory = _ => messageStore,
};

var agent = client.GetChatClient(deploymentName)
    .CreateAIAgent(agentOptions)
    .AsBuilder()
    .UseOpenTelemetry(
        sourceName: "*Microsoft.Agents.AI",
        configure: cfg => cfg.EnableSensitiveData = true)
    .Build();

var thread = agent.GetNewThread();

var agentResponse = await agent.RunAsync(request.Input ?? string.Empty,thread,cancellationToken: cancellationToken);

return agentResponse.ToString();
```

### Pattern 3: Agent with RunOptions

```csharp
var agent = services.ChatClient.CreateAIAgent(agentOptions)
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "*Microsoft.Agents.AI", configure: cfg => cfg.EnableSensitiveData = true)
    .Build();

var thread = agent.GetNewThread();

var runOptions = new ChatClientAgentRunOptions(new ChatOptions
{
    Temperature = 1.0f,
    Seed = 0,
    MaxOutputTokens = services.PerformanceAnalyzerOptions.CurrentValue.MaxTokens,
});

var agentResponse = await agent.RunAsync(request.Input, thread, runOptions: runOptions, cancellationToken: cancellationToken);

return agentResponse.ToString();
```

## Tool Implementation

### Tool Naming and Structure

- **Naming**: End with `Tool` (e.g., `EntraIdUserSearchTool`, `TopNDriversQueryTool`)
- **Methods**: Public methods decorated with `[Description]` attributes
- **Parameters**: Decorated with `[Description]` for clarity to the LLM
- **Return types**: Use `Result<T>` pattern or direct return types

### Example: Search Tool with Result Pattern

```csharp
using System.ComponentModel;
using {CompanyName}.AI.Services.Commons.Patterns;

namespace {CompanyName}.AI.Services.Agents.UserProfiler.Tools;

/// <summary>
/// Tool to search for users in Microsoft Entra ID.
/// </summary>
internal sealed class EntraIdUserSearchTool
{
    private readonly GraphServiceClient graphClient;
    private readonly ILogger logger;
    private readonly int maxResults;

    public EntraIdUserSearchTool(
        GraphServiceClient graphClient,
        ILogger<EntraIdUserSearchTool> logger,
        int maxResults)
    {
        this.graphClient = graphClient;
        this.logger = logger;
        this.maxResults = maxResults;
    }

    /// <summary>
    /// Searches for users in Entra ID by name or email.
    /// </summary>
    [Description(@"Searches for users in Microsoft Entra ID by name or email and returns their profile information.")]
    public async Task<Result<IEnumerable<MemberInfo>>> SearchUserAsync(
        [Description(@"The user's display name or email address. Examples: 'John Doe', 'jdoe@company.com'")]
        string searchQuery,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation(@"Searching for users with query: {SearchQuery}", searchQuery);

            IEnumerable<User> users;

            if (IsEmailAddress(searchQuery))
            {
                users = await SearchByEmailAsync(searchQuery, cancellationToken);
            }
            else
            {
                users = await SearchByDisplayNameAsync(searchQuery, cancellationToken);
            }

            var userList = users.ToList();

            if (userList.Count == 0)
            {
                return Result<IEnumerable<MemberInfo>>.Fail($@"User not found with query: '{searchQuery}'");
            }

            var memberInfoList = userList.Select(user => new MemberInfo
            {
                UserId = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Mail,
                JobTitle = user.JobTitle,
                Department = user.Department,
            }).ToList();

            return Result<IEnumerable<MemberInfo>>.Ok(memberInfoList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, @"Error searching for users with query: {SearchQuery}", searchQuery);
            return Result<IEnumerable<MemberInfo>>.Fail(ex);
        }
    }
}
```

### Example: Query Building Tool

```csharp
using System.ComponentModel;

namespace {CompanyName}.AI.Services.Agents.PerformanceAnalyzer.Tools;

/// <summary>
/// Tool to generate DAX queries for performance analysis.
/// </summary>
public sealed class TopNDriversQueryTool
{
    /// <summary>
    /// Generates a DAX query to get the top N drivers affecting an operation.
    /// </summary>
    [Description(@"Returns a DAX query to get the top N drivers that affect an operation in a given week.")]
    public string GetTopNDriversQuery(
        [Description(@"Year in format 'YYYY' (e.g., 2023).")] int year,
        [Description(@"Week code in format 'YYYYWW' (e.g., 202313 for week 13 of 2023).")] int week,
        [Description(@"Number of top results to return (e.g., 3 for top 3).")] int topN,
        [Description(@"Operation name.")] string operation,
        [Description(@"Country name.")] string country)
    {
        var daxQuery = $@"
        DEFINE
            VAR __DS0FilterTable3 =
                TREATAS(
                    ROW(
                        ""Global Company Country Desc"", ""{country}"",
                        ""Operation Desc"", ""{operation}""
                    ),
                    'Shop'[Global Company Country Desc],
                    'Shop'[Operation Desc]
                )
            VAR __DS0FilterTable4 =
                TREATAS(
                    ROW(""Year"", {year}, ""Week Code"", {week}),
                    'Date'[Year],
                    'Date'[Week Code]
                )
            VAR __DS0Core = SUMMARIZECOLUMNS(...)
            VAR __DS0PrimaryWindowed = TOPN({topN}, __DS0Core, ...)
        EVALUATE
            __DS0PrimaryWindowed
        ORDER BY ...
        ".Trim();

        return daxQuery;
    }
}
```

### Creating AIFunctions

```csharp
// From methods
var searchFunction = AIFunctionFactory.Create(entraIdTool.SearchUserAsync);
var queryFunction = AIFunctionFactory.Create(queryTool.GetTopNDriversQuery);

// Add to agent's tools
var agentOptions = new ChatClientAgentOptions
{
    ChatOptions = new ChatOptions
    {
        Tools = [searchFunction, queryFunction],
    },
};
```

## Options and Configuration

```csharp
using System.ComponentModel.DataAnnotations;

namespace {CompanyName}.AI.Services.Agents.{AgentName}.Options;

public sealed class {AgentName}Options
{
    [Required]
    public required string DeploymentName { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public required int MaxTokens { get; init; }

    [Required]
    public required string CosmosDBContainerName { get; init; }
}
```

**appsettings.json:**

```json
{
  "{AgentName}Options": {
    "DeploymentName": "gpt-4o",
    "MaxTokens": 4096,
    "CosmosDBContainerName": "agent-conversations"
  }
}
```

## Services Record Pattern

```csharp
using Microsoft.Agents.AI;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace {CompanyName}.AI.Services.Agents.{AgentName}.Models;

/// <summary>
/// Services required by the {AgentName} AI controller.
/// </summary>
/// <param name="ChatClient">Configured AI chat client for agent interactions.</param>
/// <param name="LoggerFactory">Logger factory for creating logger instances.</param>
/// <param name="{AgentName}Options">Configuration options for the agent.</param>
/// <param name="MessageStoreFactory">Factory for creating chat message stores per conversation.</param>
/// <param name="{ServiceName}">Domain-specific service dependencies.</param>
/// <param name="Logger">Logger instance for the controller.</param>
/// <param name="TelemetryClient">Application Insights telemetry client.</param>
public sealed record {AgentName}AIControllerServices(
    IChatClient ChatClient,
    ILoggerFactory LoggerFactory,
    IOptionsMonitor<{AgentName}Options> {AgentName}Options,
    Func<string, Task<CosmosChatMessageStore>> MessageStoreFactory,
    I{ServiceName} {ServiceName},
    ILogger<{AgentName}AIControllerServices> Logger,
    TelemetryClient TelemetryClient);
```

## Agent Instructions Guidelines

Instructions should be clear, step-by-step, using "MUST" for required behaviors, with examples and tool usage guidance.

### Example: Comprehensive Instructions

```csharp
var instructions = """
    You are an AI Agent that answers performance questions about operations using DAX queries.

    You MUST:
        1) ALWAYS call GetOperationCatalogAsync first to retrieve the list of valid operations and countries.
        Pick the best match from the catalog. If ambiguous or missing, ask a clarifying question.

        2) Extract required parameters from the user's question:
        - Operation name (must match catalog exactly)
        - Country name (must match catalog exactly)
        - Year and week in format YYYYWW (e.g., 202313 for week 13 of 2023)

        3) If any required parameter is missing or ambiguous, ask a SHORT clarifying question.
        Example: "Which operation are you asking about: Operation A or Operation B?"

        4) Determine the user's intent (e.g., top N drivers, evolution of sales, impact comparison).

        5) Call the appropriate query building tool (GetTopNDriversQuery, GetEvolutionOfSalesQuery, etc.)
        to generate the DAX query with the extracted parameters.

        6) ALWAYS execute the generated DAX query by calling ExecuteDaxQueryAsync. This step is MANDATORY.
        Never skip query execution or provide a response without executing the query.

        7) Read the JSON results returned from ExecuteDaxQueryAsync and formulate a clear, concise answer
        using that data. Include specific numbers, trends, and insights from the results.

        8) End your response with 2-4 relevant follow-up questions the user might want to ask,
        pre-filled with the same operation/country/week when applicable.

    IMPORTANT:
        - Always respond in the same language as the user's question
        - If the query returns no results, explain this clearly and suggest alternative questions
        - Be conversational and helpful, not robotic
        - Cite specific data points from query results in your answer

    FORMAT YOUR RESPONSE:
        1. Direct answer to the user's question with data
        2. Key insights or trends
        3. Suggested follow-up questions
    """;
```

### For Catalog-Based Agents

```csharp
var instructions = """
    You are a helpful assistant for {domain}.

    WORKFLOW:
        1) ALWAYS call the catalog function first to get the list of valid values
        2) Match the user's input to the closest item in the catalog
        3) If multiple matches or ambiguous, ask for clarification
        4) If no match found, list available options and ask user to choose
        5) Once you have the exact value, proceed with the tool that requires it

    MATCHING RULES:
        - Use fuzzy matching for user input (handle typos, abbreviations)
        - Be case-insensitive when comparing
        - If confidence is low (<80%), ask for confirmation
        - Present at most 3-5 options when asking for clarification

    Always respond in the user's language.
    """;
```

## Error Handling

```csharp
public async Task<IActionResult> AnswerAsync([FromBody] AgentRequest request, CancellationToken cancellationToken)
{
    try
    {
        // Validate headers
        if (!HttpContext.TryGetRequestHeaderValue(CommonConstants.HttpHeaders.ConversationId, out var conversationId))
        {
            return BadRequest($@"Missing required header: {CommonConstants.HttpHeaders.ConversationId}");
        }

        // Agent execution
        var result = await ExecuteAgentAsync(request, conversationId, cancellationToken);
        return Ok(result);
    }
    catch (OperationCanceledException)
    {
        return StatusCode(StatusCodes.Status408RequestTimeout, @"Request was cancelled");
    }
    catch (TimeoutException)
    {
        return StatusCode(StatusCodes.Status408RequestTimeout, @"Request timed out");
    }
    catch (InvalidOperationException ioe)
    {
        services.Logger.LogError(ioe, @"{Method} invalid operation", nameof(AnswerAsync));
        return BadRequest(ioe.Message);
    }
    catch (Exception exception)
    {
        services.Logger.LogError(exception, @"{Method} error! Request: {Input}", nameof(AnswerAsync), request.Input);
        throw; // Let global error handler process
    }
}
```

**Configuration validation at startup:**

```csharp
var containerId = builder.Configuration[@"{AgentName}Options:CosmosDBContainerName"];
if (string.IsNullOrWhiteSpace(containerId))
{
    throw new InvalidOperationException(@"Missing config: {AgentName}Options:CosmosDBContainerName");
}

// Validate agent responses
var responseText = agentResponse.ToString();
if (string.IsNullOrWhiteSpace(responseText))
{
    throw new InvalidOperationException(@"Agent returned empty response");
}
```

## Telemetry and Observability

```csharp
// OpenTelemetry integration in agent builder
var agent = services.ChatClient.CreateAIAgent(agentOptions)
    .AsBuilder()
    .UseOpenTelemetry(
        sourceName: @"*Microsoft.Agents.AI",
        configure: cfg => cfg.EnableSensitiveData = true) // Only in development
    .Build();

// Custom events
services.TelemetryClient.TrackEvent(
    Constants.TelemetryEvents.AgentRequestStarted,
    properties: new Dictionary<string, string>
    {
        [@"ConversationId"] = conversationId,
        [@"AgentName"] = "{AgentName}",
    });

// Structured logging
services.Logger.LogInformation(
    @"Agent {AgentName} processing. ConversationId: {ConversationId}",
    agentName, conversationId);
```

## Common Patterns and Anti-Patterns

### DO

- Use descriptive, domain-specific agent names
- Provide clear, step-by-step instructions to agents
- Validate all inputs and configuration at startup
- Use message stores for conversation persistence
- Implement proper error handling and logging
- Use `AIFunctionFactory.Create()` for creating functions from methods
- Use OpenTelemetry for observability
- Test agents with real LLM interactions
- Document tool methods with `[Description]` attributes
- Use services record pattern for dependency injection

### DON'T

- Don't create agents with more than 15-20 tools (use multi-agent patterns instead)
- Don't hardcode configuration values
- Don't skip conversation ID validation
- Don't log sensitive data or credentials
- Don't use agents for deterministic, structured tasks (use regular code)
- Don't forget to execute queries after generating them
- Don't ignore agent response validation
- Don't create agents without clear instructions
- Don't skip OpenTelemetry integration
- Don't use synchronous operations in async methods

## Validation Checklist

Before considering an AI Agent project complete:

- [ ] Agent has clear, comprehensive instructions
- [ ] All tools have descriptive `[Description]` attributes
- [ ] Configuration options use data annotations for validation
- [ ] Message store factory is registered for conversation persistence
- [ ] Required headers are validated
- [ ] Error handling covers cancellation, timeouts, and exceptions
- [ ] OpenTelemetry integration is configured
- [ ] Swagger documentation is complete and accurate
- [ ] Custom telemetry events are tracked
- [ ] Agent responses are validated before returning
- [ ] Sensitive data is not logged or exposed
- [ ] README documents agent purpose and usage

## References

- [Microsoft Agent Framework Overview](https://learn.microsoft.com/en-us/agent-framework/overview/agent-framework-overview)
- [Microsoft Agent Framework GitHub](https://github.com/microsoft/agent-framework)

> **When in doubt, prefer the simpler approach, ask the user or request better specifications**
