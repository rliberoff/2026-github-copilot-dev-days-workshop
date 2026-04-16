using TodoLite.Core;

var filePath = "todolite.json";

// Parse --file option
var argList = args.ToList();
var fileIndex = argList.IndexOf("--file");
if (fileIndex >= 0 && fileIndex + 1 < argList.Count)
{
    filePath = argList[fileIndex + 1];
    argList.RemoveAt(fileIndex);
    argList.RemoveAt(fileIndex);
}

var repository = new JsonFileTodoRepository(filePath);
var app = new TodoApp(repository);

if (argList.Count == 0)
{
    PrintUsage();
    return 2;
}

var command = argList[0].ToLowerInvariant();

switch (command)
{
    case "add":
        return HandleAdd(argList);

    case "list":
        return HandleList(argList);

    case "done":
        return HandleDone(argList);

    case "rm":
        return HandleRm(argList);

    case "help":
    case "--help":
    case "-h":
        PrintUsage();
        return 2;

    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        PrintUsage();
        return 2;
}

int HandleAdd(List<string> arguments)
{
    if (arguments.Count < 2)
    {
        Console.Error.WriteLine("Usage: todolite add <text...>");
        return 2;
    }

    var text = string.Join(' ', arguments.Skip(1));
    var item = app.Add(text);
    Console.WriteLine($"Created task #{item.Id}: {item.Text}");
    return 0;
}

int HandleList(List<string> arguments)
{
    var openOnly = arguments.Contains("--open");
    var items = app.List(openOnly);
    foreach (var item in items)
    {
        var check = item.IsDone ? "x" : " ";
        Console.WriteLine($"[{check}] {item.Id,3}  {item.Text}");
    }

    return 0;
}

int HandleDone(List<string> arguments)
{
    if (arguments.Count < 2 || !int.TryParse(arguments[1], out var id))
    {
        Console.Error.WriteLine("Usage: todolite done <id>");
        return 2;
    }

    if (app.MarkDone(id))
    {
        Console.WriteLine($"OK: task #{id} marked as done.");
        return 0;
    }

    Console.WriteLine($"Task #{id} does not exist.");
    return 1;
}

int HandleRm(List<string> arguments)
{
    if (arguments.Count < 2 || !int.TryParse(arguments[1], out var id))
    {
        Console.Error.WriteLine("Usage: todolite rm <id>");
        return 2;
    }

    if (app.Remove(id))
    {
        Console.WriteLine($"OK: task #{id} removed.");
        return 0;
    }

    Console.WriteLine($"Task #{id} does not exist.");
    return 1;
}

void PrintUsage()
{
    Console.WriteLine("""
        Usage: todolite <command> [arguments] [--file <path>]

        Commands:
          add <text...>     Create a new task
          list [--open]     List all tasks (or only open tasks)
          done <id>         Mark a task as done
          rm <id>           Remove a task
          help              Show this help message
        """);
}
