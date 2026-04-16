namespace TodoLite.Core;

/// <summary>
/// Represents a single task in the to-do list.
/// </summary>
public sealed record TodoItem(
    int Id,
    string Text,
    bool IsDone,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DoneAtUtc);
