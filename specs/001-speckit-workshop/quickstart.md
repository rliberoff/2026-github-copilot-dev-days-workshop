# Quickstart: Spec Kit Introductory Workshop

**Feature**: `001-speckit-workshop`
**Date**: 2026-04-13

## Prerequisites

| Tool | Minimum Version | Verification Command |
|------|----------------|---------------------|
| Git | 2.x | `git --version` |
| Python | 3.11+ | `python --version` |
| uv | latest | `uv --version` |
| specify CLI | 0.6.x+ | `specify version` |
| .NET SDK | 10.0+ | `dotnet --version` |
| VS Code with GitHub Copilot | latest | Copilot icon in VS Code activity bar |

## Quick Verification

```bash
# Run all checks at once
specify check
```

## Greenfield Exercise (C#)

```bash
cd exercises/greenfield/csharp
dotnet build
dotnet test
dotnet run --project src/TodoLite.Cli -- add "Buy milk"
dotnet run --project src/TodoLite.Cli -- list
```

**Expected output after list**:

```
[ ]   1  Buy milk
```

## Greenfield Exercise (Python)

```bash
cd exercises/greenfield/python
pip install -e ".[dev]"
pytest
python -m todolite add "Buy milk"
python -m todolite list
```

**Expected output after list**:

```
[ ]   1  Buy milk
```

## Brownfield Exercise (C#)

```bash
cd exercises/brownfield/csharp
dotnet build
dotnet run --project src/Notes.Api
```

In a separate terminal:

```bash
# Create a note
curl -s -X POST "http://localhost:5000/notes" \
  -H "Content-Type: application/json" \
  -d "{\"title\":\"Buy milk\",\"body\":\"Semi-skimmed\",\"tags\":[\"groceries\"]}"

# Search by text
curl -s "http://localhost:5000/notes/search?q=milk"
```

**Expected**: 201 Created with note JSON, then search returns 1 result.

## Brownfield Exercise (Python)

```bash
cd exercises/brownfield/python
pip install -e ".[dev]"
pytest
python -m notes_api
```

In a separate terminal:

```bash
# Same curl commands as C# brownfield
```

## Preset and Extension

```bash
# From project root with Spec Kit initialized
specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5
specify preset list
specify preset resolve spec-template

specify extension add --dev exercises/extensions/my-ext
specify extension list
specify extension info my-ext
```

## Common Issues

| Issue | Fix |
|-------|-----|
| `command not found: specify` | Add `~/.local/bin` to PATH, or reinstall with `uv tool install specify-cli --force` |
| `command not found: uv` | Install uv: `curl -LsSf https://astral.sh/uv/install.sh \| sh` (Linux/macOS) or `powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 \| iex"` (Windows) |
| Slash commands not showing | Restart agent, re-run `specify init --here --ai copilot --force` |
| Python < 3.11 | Install Python 3.11+ or use `uv python install 3.11` |
| .NET SDK not found | Install from <https://dotnet.microsoft.com/download> |
