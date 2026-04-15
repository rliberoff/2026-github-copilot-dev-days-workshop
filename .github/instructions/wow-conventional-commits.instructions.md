---
applyTo: "**"
description: "Apply these instructions only when generating a commit message for source code changes and configuration control. Ignore them in all other contexts. Follow the 'Conventional Commits' specification to produce consistent, readable commit histories and to support automated changelog generation and continuous integration workflows."
---

# Conventional Commits Guidelines

Guidelines for writing commit messages following the [Conventional Commits](https://www.conventionalcommits.org/) specification to ensure a readable history, automate changelog generation, and facilitate continuous integration.

## Project Context

- **Scope**: All project commits, including automated commits from GitHub Copilot or other tools
- **Language**: All commit messages must be in English

## Commit Message Format

The commit message must be structured as follows:

```text
<type> - [optional scope]: <description>
```

### Components

- **type**: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`
- **scope** (optional): The part of the code concerned (e.g., `api`, `domain`, `infrastructure`, `tests`)
- **description**: Short imperative description, no initial capital letter, no period at the end
- **Character limit**: First line must not exceed 72 characters

### Examples

- `feat - api: add order endpoint`
- `fix - (domain): correct order validation logic`
- `test - (order): add unit tests for order creation`
- `chore: update dependencies`

## Best Practices

- **One commit = one logical change**: Each commit should represent a single, atomic unit of work.
- **Use scope**: Specify the affected layer or feature to provide context.
- **Breaking changes**: Add `!` after the type or scope and detail in the commit body.

## References

- [Conventional Commits v1.0.0](https://www.conventionalcommits.org/)
