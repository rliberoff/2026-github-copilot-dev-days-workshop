---
name: speckit.specify
description: "Generate a feature specification using the preset's spec template with Verifiable Acceptance Criteria (dotnet-workshop-lite preset override)"
---

## Preset Override: speckit.specify

This command override is provided by the `dotnet-workshop-lite` preset. It modifies the default `/speckit.specify` template resolution so the spec is generated from the preset's custom template.

### Template Resolution Override

When creating the spec file (step 3 of the specify workflow), do **not** copy `.specify/templates/spec-template.md` directly. Instead, resolve the spec template using the following priority stack (highest priority wins):

1. **Overrides** (local): `.specify/overrides/templates/spec-template.md`
2. **Presets** (by priority — lower number wins): scan `.specify/presets/.registry` for enabled presets, sort by `priority` ascending, and check `.specify/presets/<preset-id>/templates/spec-template.md` for each
3. **Extensions**: scan `.specify/extensions/.registry` for enabled extensions and check `.specify/extensions/<extension-id>/templates/spec-template.md` for each
4. **Core**: `.specify/templates/spec-template.md`

Pick the **first file that exists** following this order and use it as the starting template for `spec.md`.

Apply the same resolved template when loading the template to understand required sections (step 4).

### Additional Instructions

When generating the specification, you MUST include a **Verifiable Acceptance Criteria** section with a table in the following format:

| ID | Requirement | Verification Command | Expected Output | Pass/Fail |
|----|-------------|---------------------|-----------------|-----------|
| VAC-001 | {requirement description} | {command or step to verify} | {expected result} | [ ] |

Populate this table with every acceptance criterion derived from the feature description. Each row must include:

- **ID**: Sequential identifier starting with `VAC-001`.
- **Requirement**: A concrete, testable requirement statement.
- **Verification Command**: The exact command, API call, or manual step to verify the requirement.
- **Expected Output**: The expected result that constitutes a pass.
- **Pass/Fail**: Checkbox for tracking verification status.

This ensures that acceptance criteria are concrete, verifiable, and traceable.
