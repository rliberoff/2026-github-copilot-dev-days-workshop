using Xunit;

using TodoLite.Core;

namespace TodoLite.Core.Tests;

public sealed class TodoAppTests : IDisposable
{
    private readonly string tempFile;
    private readonly TodoApp app;

    public TodoAppTests()
    {
        tempFile = Path.Combine(Path.GetTempPath(), $"todolite-test-{Guid.NewGuid()}.json");
        var repository = new JsonFileTodoRepository(tempFile);
        app = new TodoApp(repository);
    }

    public void Dispose()
    {
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Add_CreatesTask()
    {
        var item = app.Add("Buy milk");

        Assert.Equal(1, item.Id);
        Assert.Equal("Buy milk", item.Text);
        Assert.False(item.IsDone);
        Assert.Null(item.DoneAtUtc);
    }

    [Fact]
    public void List_ReturnsAllTasks()
    {
        app.Add("Task one");
        app.Add("Task two");

        var items = app.List();

        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void List_OpenOnly_FiltersDoneTasks()
    {
        app.Add("Open task");
        var done = app.Add("Done task");
        app.MarkDone(done.Id);

        var items = app.List(openOnly: true);

        Assert.Single(items);
        Assert.Equal("Open task", items[0].Text);
    }

    [Fact]
    public void MarkDone_MarksTaskAsComplete()
    {
        var item = app.Add("Test task");

        var result = app.MarkDone(item.Id);

        Assert.True(result);
        var items = app.List();
        Assert.True(items[0].IsDone);
        Assert.NotNull(items[0].DoneAtUtc);
    }

    [Fact]
    public void MarkDone_IsIdempotent()
    {
        var item = app.Add("Test task");

        app.MarkDone(item.Id);
        var result = app.MarkDone(item.Id);

        Assert.True(result);
    }

    [Fact]
    public void Remove_RemovesTask()
    {
        var item = app.Add("Test task");

        var result = app.Remove(item.Id);

        Assert.True(result);
        Assert.Empty(app.List());
    }

    [Fact]
    public void MarkDone_NotFound_ReturnsFalse()
    {
        var result = app.MarkDone(999);

        Assert.False(result);
    }

    [Fact]
    public void Remove_NotFound_ReturnsFalse()
    {
        var result = app.Remove(999);

        Assert.False(result);
    }

    [Fact]
    public void Add_EmptyText_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => app.Add("   "));
    }
}
