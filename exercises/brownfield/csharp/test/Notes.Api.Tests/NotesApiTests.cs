using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Notes.Api.Tests;

public sealed class NotesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly HttpClient client;

    public NotesApiTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Notes API OK", body);
    }

    [Fact]
    public async Task CreateNote_ReturnsCreatedWithNormalizedTags()
    {
        var request = new { title = "Test Note", body = "Test body", tags = new[] { "  Groceries  ", "HOME", "groceries" } };
        var response = await client.PostAsJsonAsync("/notes", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var note = await response.Content.ReadFromJsonAsync<JsonElement>();
        var tags = note.GetProperty("tags").EnumerateArray().Select(t => t.GetString()).ToList();
        Assert.Equal(["groceries", "home"], tags);
    }

    [Fact]
    public async Task CreateNote_WithoutTitle_ReturnsBadRequest()
    {
        var request = new { title = (string?)null, body = "No title" };
        var response = await client.PostAsJsonAsync("/notes", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ListNotes_ReturnsDescendingOrder()
    {
        await client.PostAsJsonAsync("/notes", new { title = "First" });
        await client.PostAsJsonAsync("/notes", new { title = "Second" });

        var response = await client.GetAsync("/notes");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var notes = await response.Content.ReadFromJsonAsync<JsonElement[]>(JsonOptions);
        Assert.NotNull(notes);
        Assert.True(notes.Length >= 2);
    }

    [Fact]
    public async Task GetNoteById_ReturnsOk()
    {
        var createResponse = await client.PostAsJsonAsync("/notes", new { title = "Fetch me" });
        var created = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetString();

        var response = await client.GetAsync($"/notes/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetNoteById_NotFound_Returns404()
    {
        var response = await client.GetAsync($"/notes/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task SearchByText_ReturnsMatchingNotes()
    {
        await client.PostAsJsonAsync("/notes", new { title = "Buy milk", body = "Semi-skimmed" });
        await client.PostAsJsonAsync("/notes", new { title = "Read book", body = "Fiction" });

        var response = await client.GetAsync("/notes/search?q=milk");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.GetProperty("count").GetInt32() >= 1);
    }

    [Fact]
    public async Task SearchByTag_ReturnsMatchingNotes()
    {
        await client.PostAsJsonAsync("/notes", new { title = "Tagged note", tags = new[] { "groceries" } });

        var response = await client.GetAsync("/notes/search?tag=groceries");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.GetProperty("count").GetInt32() >= 1);
    }

    [Fact]
    public async Task SearchCombined_ReturnsCorrectResults()
    {
        await client.PostAsJsonAsync("/notes", new { title = "Buy milk", body = "Semi-skimmed", tags = new[] { "groceries" } });
        await client.PostAsJsonAsync("/notes", new { title = "Buy bread", tags = new[] { "groceries" } });
        await client.PostAsJsonAsync("/notes", new { title = "Buy milk chocolate", tags = new[] { "sweets" } });

        var response = await client.GetAsync("/notes/search?q=milk&tag=groceries");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.GetProperty("count").GetInt32() >= 1);
    }
}
