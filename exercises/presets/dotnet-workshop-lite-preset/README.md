# .NET Workshop Lite Preset

A lightweight Spec Kit preset designed for .NET workshop exercises. It customizes the specification and implementation plan templates to enforce structured acceptance criteria and decision tracking.

## Purpose

This preset modifies two Spec Kit templates:

- **spec-template.md**: Adds a "Verifiable Acceptance Criteria" section with a pass/fail checklist format, ensuring every requirement has a testable verification step.
- **plan-template.md**: Adds a "Key Decisions" table (Decision, Options Considered, Chosen, Rationale), making architectural choices explicit and traceable.

## Installation

```bash
specify preset add --dev <path-to-this-directory> --priority 5
```

## Verification

```bash
# Confirm the preset is installed
specify preset list

# Confirm the spec template includes the custom section
specify preset resolve spec-template

# Confirm the plan template includes the decisions table
specify preset resolve plan-template
```

## Impact

After installation, any new specification or plan generated via `/speckit.specify` or `/speckit.plan` will include the additional sections defined by this preset. Existing artifacts are not modified.

## Rollback

To remove the preset and revert to the default templates:

```bash
specify preset remove dotnet-workshop-lite
```

After removal, `specify preset list` will no longer show this preset, and `specify preset resolve` will return the next-priority template (extension, then core).

## Structure

```text
dotnet-workshop-lite-preset/
├── preset.yml              # Manifest
├── README.md               # This file
├── LICENSE                  # MIT License
├── templates/
│   ├── spec-template.md    # Customized spec template
│   └── plan-template.md    # Customized plan template
└── commands/
    └── speckit.plan.md     # Plan command override with decisions table prompt
```

## License

MIT — see [LICENSE](LICENSE).
