using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

var store = new ConcurrentDictionary<Guid, Note>();

// GET / — Health check
app.MapGet("/", () => Results.Ok("Notes API OK"));

// GET /notes — List all notes
app.MapGet("/notes", () =>
{
    var notes = store.Values
        .OrderByDescending(n => n.CreatedAtUtc)
        .ToList();
    return Results.Ok(notes);
});

// GET /notes/{id} — Get note by ID
app.MapGet("/notes/{id:guid}", (Guid id) =>
{
    if (store.TryGetValue(id, out var note))
    {
        return Results.Ok(note);
    }

    return Results.NotFound(new { message = "Note not found." });
});

// POST /notes — Create a new note
app.MapPost("/notes", (CreateNoteRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest(new { message = "Title is required." });
    }

    var id = Guid.NewGuid();
    var tags = NormalizeTags(request.Tags);
    var note = new Note(id, request.Title.Trim(), request.Body?.Trim() ?? string.Empty, tags, DateTimeOffset.UtcNow);
    store[id] = note;

    return Results.Created($"/notes/{id}", note);
});

app.Run();

static List<string> NormalizeTags(List<string>? tags)
{
    if (tags is null || tags.Count == 0)
    {
        return [];
    }

    return tags
        .Select(t => t.Trim().ToLowerInvariant())
        .Where(t => t.Length > 0)
        .Distinct()
        .Order()
        .ToList();
}

/// <summary>
/// Represents a note in the store.
/// </summary>
public sealed record Note(
    Guid Id,
    string Title,
    string Body,
    List<string> Tags,
    DateTimeOffset CreatedAtUtc);

/// <summary>
/// Request DTO for creating a note.
/// </summary>
public sealed record CreateNoteRequest(
    string? Title,
    string? Body,
    List<string>? Tags);

/// <summary>
/// Entry point marker for WebApplicationFactory test support.
/// </summary>
public partial class Program { }
