namespace TodoLite.Core;

/// <summary>
/// Application service for managing to-do items.
/// </summary>
public sealed class TodoApp
{
    private readonly JsonFileTodoRepository repository;

    public TodoApp(JsonFileTodoRepository repository)
    {
        this.repository = repository;
    }

    /// <summary>
    /// Creates a new task with the given text.
    /// </summary>
    public TodoItem Add(string text)
    {
        var trimmed = text.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ArgumentException("Task text must not be empty.", nameof(text));
        }

        var items = repository.Load();
        var id = repository.NextId(items);
        var item = new TodoItem(id, trimmed, false, DateTimeOffset.UtcNow, null);
        items.Add(item);
        repository.Save(items);
        return item;
    }

    /// <summary>
    /// Lists all tasks, or only open tasks if <paramref name="openOnly"/> is true.
    /// </summary>
    public List<TodoItem> List(bool openOnly = false)
    {
        var items = repository.Load();
        if (openOnly)
        {
            return items.Where(i => !i.IsDone).OrderBy(i => i.Id).ToList();
        }

        return items.OrderBy(i => i.Id).ToList();
    }

    /// <summary>
    /// Marks a task as done. Idempotent — marking an already-done item returns true.
    /// </summary>
    /// <returns>True if the task exists, false otherwise.</returns>
    public bool MarkDone(int id)
    {
        var items = repository.Load();
        var index = items.FindIndex(i => i.Id == id);
        if (index < 0)
        {
            return false;
        }

        var item = items[index];
        if (!item.IsDone)
        {
            items[index] = item with { IsDone = true, DoneAtUtc = DateTimeOffset.UtcNow };
            repository.Save(items);
        }

        return true;
    }

    /// <summary>
    /// Removes a task permanently.
    /// </summary>
    /// <returns>True if the task existed and was removed, false otherwise.</returns>
    public bool Remove(int id)
    {
        var items = repository.Load();
        var removed = items.RemoveAll(i => i.Id == id);
        if (removed > 0)
        {
            repository.Save(items);
            return true;
        }

        return false;
    }
}
