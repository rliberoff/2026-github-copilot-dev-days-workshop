---
name: speckit.plan
description: "Generate an implementation plan with a Key Decisions table (dotnet-workshop-lite preset override)"
---

## Preset Override: speckit.plan

This command override is provided by the `dotnet-workshop-lite` preset. It extends the default `/speckit.plan` behavior by including an explicit prompt for the Key Decisions table.

### Additional Instructions

When generating the implementation plan, you MUST include a **Key Decisions** section with a table in the following format:

| Decision | Options Considered | Chosen | Rationale |
|----------|--------------------|--------|-----------|
| {decision description} | {option A, option B, ...} | {chosen option} | {why this option was selected} |

Populate this table with every significant technical or architectural decision made during planning. Each row must include:

- **Decision**: What needed to be decided (e.g., "persistence strategy", "test framework", "API style").
- **Options Considered**: All viable alternatives evaluated.
- **Chosen**: The selected option.
- **Rationale**: Why this option was chosen over the alternatives.

This ensures that design decisions are explicit, traceable, and reviewable by the team.
