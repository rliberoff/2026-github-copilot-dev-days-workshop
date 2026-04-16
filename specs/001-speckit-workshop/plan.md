# Implementation Plan: Spec Kit Introductory Workshop

**Branch**: `001-speckit-workshop` | **Date**: 2026-04-13 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/001-speckit-workshop/spec.md`

## Summary

Deliver a 2-hour introductory workshop on Spec-Driven Development (SDD) with GitHub Spec Kit. The implementation produces: 4 standalone executable reference projects (C# and Python × greenfield and brownfield), workshop instructional materials in Spanish/Markdown, a preset example, an extension example, and troubleshooting guidance. All code and SDD artifacts are in English; participant-facing materials are in Spanish.

## Technical Context

**Language/Version**: C# 14 / .NET 10 and Python 3.11+
**Primary Dependencies**: C#: xUnit, ASP.NET Minimal API; Python: stdlib (argparse, json, dataclasses), Flask 3.x, pytest
**Storage**: JSON file (greenfield), in-memory dict/ConcurrentDictionary (brownfield)
**Testing**: C#: `dotnet test` (xUnit); Python: `pytest`
**Target Platform**: Windows, macOS, Linux (cross-platform workshop)
**Project Type**: Workshop content repository containing 4 reference projects (2 CLI/library, 2 web-service) plus instructional materials
**Performance Goals**: N/A (didactic applications; no production performance targets)
**Constraints**: Each exercise verifiable in under 10 minutes; total workshop fits in ~120 minutes
**Scale/Scope**: 4 reference projects, 6 workshop modules, ~15 instructional/troubleshooting documents

## Constitution Check (Pre-Phase 0)

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] SDD flow complete: greenfield and brownfield exercise guides instruct participants to perform the full core flow live (constitution → specify → plan → tasks → implement), plus optional steps (clarify, analyze, checklist). The customization lab (presets/extensions) exercises selected SDD commands to validate customization effects but is not required to complete the full flow end-to-end.
- [x] Dual coverage: functionally equivalent strategy defined for C# and Python (same objectives, same acceptance criteria, comparable commands). See research.md RT-01, RT-02.
- [x] Executable verification: FR-008 requires exercise-appropriate executable verification steps with documented expected output. Code-producing exercises (US1, US3) use build/test/run; the customization exercise (US2) uses install/resolve/quickcheck. All expected outputs are documented in spec.md § Validation Commands and § Presets/Extensions Verification.
- [x] Scenario coverage: plan includes greenfield (45 min) and brownfield (25 min) within 2-hour envelope.
- [x] Responsible customization: preset and extension documented with purpose, impact, and rollback (uninstall via `specify preset remove` / `specify extension remove`).
- [x] Technical sources: decisions backed by official Spec Kit repository documentation and .NET/Python official docs.

## Project Structure

### Documentation (this feature)

```text
specs/001-speckit-workshop/
├── plan.md              # This file
├── research.md          # Phase 0 output (completed)
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── greenfield-cli.md
│   └── brownfield-api.md
├── checklists/
│   └── requirements.md  # Specification quality checklist
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
exercises/
├── greenfield/
│   ├── csharp/
│   │   ├── TodoLite.sln
│   │   ├── src/
│   │   │   ├── TodoLite.Core/
│   │   │   │   ├── TodoLite.Core.csproj
│   │   │   │   ├── TodoItem.cs
│   │   │   │   ├── JsonFileTodoRepository.cs
│   │   │   │   └── TodoApp.cs
│   │   │   └── TodoLite.Cli/
│   │   │       ├── TodoLite.Cli.csproj
│   │   │       └── Program.cs
│   │   └── test/
│   │       └── TodoLite.Core.Tests/
│   │           ├── TodoLite.Core.Tests.csproj
│   │           └── TodoAppTests.cs
│   └── python/
│       ├── pyproject.toml
│       ├── todolite/
│       │   ├── __init__.py
│       │   ├── __main__.py
│       │   ├── models.py
│       │   └── repository.py
│       └── tests/
│           ├── __init__.py
│           └── test_todo_app.py
├── brownfield/
│   ├── csharp/
│   │   ├── NotesApi.sln
│   │   ├── src/
│   │   │   └── Notes.Api/
│   │   │       ├── Notes.Api.csproj
│   │   │       └── Program.cs
│   │   └── test/
│   │       └── Notes.Api.Tests/
│   │           ├── Notes.Api.Tests.csproj
│   │           └── NotesApiTests.cs
│   └── python/
│       ├── pyproject.toml
│       ├── notes_api/
│       │   ├── __init__.py
│       │   ├── __main__.py
│       │   └── app.py
│       └── tests/
│           ├── __init__.py
│           └── test_notes_api.py
├── presets/
│   └── dotnet-workshop-lite-preset/
│       ├── preset.yml
│       ├── README.md
│       ├── LICENSE
│       ├── templates/
│       │   ├── spec-template.md
│       │   └── plan-template.md
│       └── commands/
│           └── speckit.plan.md
└── extensions/
    └── my-ext/
        ├── extension.yml
        ├── README.md
        ├── LICENSE
        ├── templates/
        │   └── spec-template.md
        ├── commands/
        │   ├── dotnet.quickcheck.md
        │   └── python.quickcheck.md
        └── sample/
            ├── csharp/
            │   ├── Sample.sln
            │   ├── src/
            │   │   └── Sample/
            │   │       ├── Sample.csproj
            │   │       └── Greeter.cs
            │   └── test/
            │       └── Sample.Tests/
            │           ├── Sample.Tests.csproj
            │           └── GreeterTests.cs
            └── python/
                ├── pyproject.toml
                ├── sample_greeter/
                │   ├── __init__.py
                │   └── greeter.py
                └── tests/
                    ├── __init__.py
                    └── test_greeter.py

materials/                          # Workshop instructional materials (Spanish)
├── README.md                       # Main workshop guide
├── 00-preparacion.md               # Environment setup and verification
├── 01-introduccion.md              # SDD concepts and Spec Kit overview
├── 02-ejercicio-greenfield.md      # Greenfield exercise guide
├── 03-extensiones-y-presets.md     # Customization module guide
├── 04-ejercicio-brownfield.md      # Brownfield exercise guide
├── 05-cierre.md                    # Closing retrospective guide
├── retrospectiva.md                # Fill-in retrospective artifact template
└── troubleshooting.md              # Common issues and resolutions

.specify/                           # FR-016 priority stack demo — overrides layer
└── overrides/
    └── templates/
        └── spec-template.md        # Local override with "Project Banner" header (top priority)
```

**Structure Decision**: Multi-project layout under `exercises/` organized by exercise type (greenfield/brownfield) and language (csharp/python). Each subdirectory is a self-contained, independently buildable project. Workshop materials live in `materials/` as Spanish Markdown files. Presets and extensions are under `exercises/` since they are part of the hands-on customization module. The FR-016 priority-stack demo additionally uses a local override under `.specify/overrides/templates/spec-template.md` to demonstrate the top-priority (overrides) layer; this file is created during the presets and extensions exercise (T029d) and removed as part of the rollback walkthrough (T029e). This layout was chosen per research.md RT-05.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
| :--- | :--- | :--- |
| 4 reference projects instead of 2 | Constitution Principle II requires dual C#/Python coverage for both greenfield and brownfield | 2 projects (single language) violates the non-negotiable dual-language principle |
