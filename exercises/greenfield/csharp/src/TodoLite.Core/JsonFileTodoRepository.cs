using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoLite.Core;

/// <summary>
/// Persists <see cref="TodoItem"/> instances to a JSON file.
/// </summary>
public sealed class JsonFileTodoRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly string filePath;

    public JsonFileTodoRepository(string filePath)
    {
        this.filePath = filePath;
    }

    public List<TodoItem> Load()
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<TodoItem>>(json, JsonOptions) ?? [];
    }

    public void Save(List<TodoItem> items)
    {
        var json = JsonSerializer.Serialize(items, JsonOptions);
        File.WriteAllText(filePath, json);
    }

    public int NextId(List<TodoItem> items)
    {
        return items.Count == 0 ? 1 : items.Max(i => i.Id) + 1;
    }
}
