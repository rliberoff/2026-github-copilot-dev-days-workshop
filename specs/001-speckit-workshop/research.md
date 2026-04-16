# Research: Spec Kit Introductory Workshop

**Feature**: `001-speckit-workshop`
**Date**: 2026-04-13
**Phase**: 0 — Outline & Research

## Research Tasks

### RT-01: Python framework for greenfield CLI exercise (TodoLite equivalent)

**Context**: The spec requires functionally equivalent Python and C# implementations. The C# version uses a console app with System.Text.Json persistence. The Python equivalent must be minimal, dependency-light, and learnable in a workshop setting.

**Decision**: Use the Python standard library only (`json`, `argparse`, `pathlib`, `dataclasses`). No external framework.

**Rationale**: Mirrors the C# approach (no external dependencies beyond xUnit for tests). Keeps `pip install` requirements to zero for the core app. Only `pytest` is needed as a dev dependency for tests. Maximum portability across participant machines.

**Alternatives considered**:

- `click` or `typer` for CLI parsing — rejected because it adds a dependency that the C# side does not need, and `argparse` is sufficient for 4 commands.
- `rich` for output formatting — rejected for the same reason; plain `print()` matches C# `Console.WriteLine`.

### RT-02: Python framework for brownfield API exercise (Notes API equivalent)

**Context**: The C# version uses ASP.NET Minimal API with in-memory `ConcurrentDictionary` storage. The Python equivalent must be a lightweight web framework suitable for a 25-minute exercise.

**Decision**: Use **Flask** (latest stable, 3.x). In-memory storage via a plain `dict` (single-threaded dev server; no concurrency concern for a workshop).

**Rationale**: Flask is the most widely known Python micro-framework, requires minimal boilerplate, and maps naturally to Minimal API patterns. FastAPI was considered but adds async complexity and Pydantic dependency that would require additional explanation time.

**Alternatives considered**:

- **FastAPI** — excellent but adds Pydantic models, async/await, and uvicorn as a runner; too much surface area for a 25-minute brownfield exercise.
- **Bottle** — single-file but less mainstream; participants are less likely to have prior exposure.
- **http.server** (stdlib) — too low-level for REST endpoints; would require manual routing and JSON handling.

### RT-03: Python testing framework

**Decision**: `pytest` for all Python exercises.

**Rationale**: De facto standard, zero-config for simple cases, compatible with both CLI and Flask test client patterns.

**Alternatives considered**:

- `unittest` — more verbose, less idiomatic for modern Python; no practical advantage in a workshop.

### RT-04: .NET version and SDK requirements

**Decision**: Target .NET 10 with C# 14 (per repository coding-style instructions). The `global.json` in each exercise project pins the SDK version.

**Rationale**: Aligns with the repository's `coding-style-csharp.instructions.md` which mandates .NET 10 / C# 14. Participants must have .NET 10 SDK installed.

**Alternatives considered**:

- .NET 8 LTS — rejected because the repository constitution and coding guidelines already mandate .NET 10.

### RT-05: Repository layout for 4 reference projects

**Context**: FR-018 requires standalone executable reference projects for C# and Python in both greenfield and brownfield exercises. These coexist in the workshop repository.

**Decision**: Use a top-level `exercises/` directory with language-specific subdirectories:

```
exercises/
├── greenfield/
│   ├── csharp/       # TodoLite .NET solution
│   └── python/       # todolite Python package
└── brownfield/
    ├── csharp/       # Notes.Api .NET solution
    └── python/       # notes_api Flask application
```

**Rationale**: Clear separation by exercise type and language. Each subdirectory is a self-contained project with its own build/test/run commands. No cross-contamination between exercises. Participants clone the repo and `cd` into the relevant path.

**Alternatives considered**:

- Flat top-level (`todolite-csharp/`, `todolite-python/`, `notes-api-csharp/`, `notes-api-python/`) — rejected because it does not convey the greenfield/brownfield grouping.
- Monorepo with shared solution — rejected because exercises must be independently buildable.

### RT-06: Preset and extension example scope

**Decision**: The preset (`dotnet-workshop-lite`) modifies `spec-template.md` and `plan-template.md`. The extension (`my-ext`) adds a single `dotnet.quickcheck` command. Both are language-agnostic in their manifest but the extension command targets .NET verification specifically, with a note that participants on the Python path can adapt it.

**Rationale**: Keeps the customization module focused on understanding the mechanism (manifest → install → resolve/execute) rather than building complex tooling. The SPEC.md already provides complete YAML manifests and Markdown commands for both.

**Alternatives considered**:

- Dual-language extensions (one for .NET, one for Python) — rejected because it doubles the teaching surface without proportional learning gain; the mechanism is identical.

### RT-07: Workshop materials language and format

**Decision**: All participant-facing materials (guides, READMEs, troubleshooting) in Spanish and Markdown. All code, SDD artifacts, commit messages, and code comments in English.

**Rationale**: Constitution Principle VI (non-negotiable).

## Summary of Resolved Unknowns

| Unknown | Resolution |
|---------|-----------|
| Python greenfield framework | stdlib only (argparse, json, dataclasses) + pytest |
| Python brownfield framework | Flask 3.x + pytest |
| .NET version | .NET 10 / C# 14 |
| Repository layout for 4 projects | `exercises/{greenfield,brownfield}/{csharp,python}` |
| Preset/extension scope | One preset (templates), one extension (single command) |
| Language policy | English for code/SDD artifacts, Spanish for materials |

All NEEDS CLARIFICATION items are resolved. Proceeding to Phase 1.
