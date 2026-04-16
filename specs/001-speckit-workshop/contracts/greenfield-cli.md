# Contract: Greenfield CLI (TodoLite)

**Feature**: `001-speckit-workshop`
**Date**: 2026-04-13

## Interface Type

Command-line interface (CLI) — invoked via `dotnet run` (C#) or `python -m todolite` (Python).

## Command Schema

```
todolite <command> [arguments] [--file <path>]
```

### Global Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `--file` | string | `todolite.json` | Path to the JSON persistence file |

### Commands

#### `add <text...>`

Creates a new task with the given text.

- **Input**: One or more words joined as the task text.
- **Output**: `Created task #<id>: <text>`
- **Exit code**: 0 on success.
- **Error**: Exit code 2 if no text provided (prints usage).

#### `list [--open]`

Lists all tasks, or only open tasks if `--open` is specified.

- **Output format**: One line per task: `[x] <id>  <text>` (done) or `[ ] <id>  <text>` (open). Items ordered by ID ascending.
- **Exit code**: 0.

#### `done <id>`

Marks a task as completed.

- **Input**: Integer task ID.
- **Output**: `OK: task #<id> marked as done.` or `Task #<id> does not exist.`
- **Exit code**: 0 on success, 1 if not found, 2 on invalid input.

#### `rm <id>`

Removes a task permanently.

- **Input**: Integer task ID.
- **Output**: `OK: task #<id> removed.` or `Task #<id> does not exist.`
- **Exit code**: 0 on success, 1 if not found, 2 on invalid input.

#### `help` / `--help` / `-h`

Prints usage information.

- **Exit code**: 2.

## Verification Commands

### C\#

```bash
cd exercises/greenfield/csharp
dotnet build
dotnet test
dotnet run --project src/TodoLite.Cli -- add "Buy milk"
dotnet run --project src/TodoLite.Cli -- list
dotnet run --project src/TodoLite.Cli -- done 1
dotnet run --project src/TodoLite.Cli -- rm 1
```

### Python

```bash
cd exercises/greenfield/python
pip install -e ".[dev]"
pytest
python -m todolite add "Buy milk"
python -m todolite list
python -m todolite done 1
python -m todolite rm 1
```

## Behavioral Parity

Both C# and Python implementations MUST:

- Produce identical exit codes for the same scenarios.
- Use the same JSON file format (array of objects with the same field names, camelCase).
- Print equivalent output messages (English).
