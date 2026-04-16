# Workshop Quick Check Extension

A Spec Kit extension that provides two slash commands for rapid project verification and a spec template with a "Risk Assessment" section.

## Purpose

This extension demonstrates Spec Kit's extensibility by adding:

- `/dotnet.quickcheck` — Runs `dotnet build` and `dotnet test` against a .NET project and reports a pass/fail summary.
- `/python.quickcheck` — Runs `pip install -e .` and `pytest` against a Python project and reports a pass/fail summary.
- **spec-template.md** — Appends a "Risk Assessment" section (risk, likelihood, mitigation) to generated specifications.

## Installation

```bash
specify extension add --dev <path-to-this-directory>
```

## Verification

```bash
# Confirm the extension is installed
specify extension list

# View extension metadata
specify extension info my-ext

# Run the .NET quick check against the sample C# project
# (from VS Code agent): /dotnet.quickcheck

# Run the Python quick check against the sample Python project
# (from VS Code agent): /python.quickcheck
```

## Sample Projects

The extension includes minimal sample projects for testing the quickcheck commands:

- **C# sample**: `sample/csharp/` — A .NET 10 solution with a `Greeter` class and one xUnit test.
- **Python sample**: `sample/python/` — A Python package with a `Greeter` class and one pytest test.

## Impact

After installation, the `/dotnet.quickcheck` and `/python.quickcheck` commands become available in the VS Code agent. The spec template adds a "Risk Assessment" section to new specifications generated via `/speckit.specify`.

## Rollback

To remove the extension:

```bash
specify extension remove my-ext
```

After removal, `specify extension list` will no longer show this extension, the slash commands will no longer be available, and the spec template will revert to the next-priority template (preset, then core).

## Structure

```text
my-ext/
├── extension.yml                   # Manifest
├── README.md                       # This file
├── LICENSE                         # MIT License
├── templates/
│   └── spec-template.md            # Spec template with Risk Assessment section
├── commands/
│   ├── dotnet.quickcheck.md        # .NET verification command
│   └── python.quickcheck.md        # Python verification command
└── sample/
    ├── csharp/                     # Sample C# project for dotnet.quickcheck
    │   ├── Sample.sln
    │   ├── src/Sample/
    │   └── test/Sample.Tests/
    └── python/                     # Sample Python project for python.quickcheck
        ├── pyproject.toml
        ├── sample_greeter/
        └── tests/
```

## License

MIT — see [LICENSE](LICENSE).
