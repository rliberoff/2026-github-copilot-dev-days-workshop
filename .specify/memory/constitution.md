<!--
Sync Impact Report
- Version change: 1.3.0 -> 1.4.0
- Modified principles:
  - Principle I restructured into three tiers:
    1. SDD-flow exercises (greenfield, brownfield): participants perform the full core flow live from a scenario premise; reference code is fallback only.
    2. Customization lab (presets/extensions): exercises selected SDD commands to validate customization effects; not required to complete the full core flow end-to-end.
    3. Content-only modules (environment verification, introduction, closing): exempt from the full SDD flow; SHOULD produce at least one visible artifact.
  - Previous version (1.3.0) treated all hands-on modules identically under a single MUST; this created a constitutional conflict (C1) because the customization lab does not repeat the full pipeline.
- Added sections:
  - None
- Removed sections:
  - None
- Templates requiring updates:
  - ⚠ .specify/templates/plan-template.md (pending: language policy gate, optional steps reference)
  - ⚠ .specify/templates/spec-template.md (pending: language policy constraint)
  - ⚠ .specify/templates/tasks-template.md (pending: language policy marker)
- Follow-up TODOs:
  - Update plan/spec/tasks templates with language policy and optional steps references
  - Verify tasks.md content-only story labels (US4, US5) are now constitutionally compliant
  - Verify US1 and US3 exercise guides include clear scenario premises for participants to generate SDD artifacts live
-->

# GitHub Spec Kit Introductory Workshop Constitution

## Core Principles

### I. SDD First and Learning Evidence

SDD-flow exercise modules (greenfield and brownfield) MUST provide sufficient guidance for participants to perform the complete Spec-Driven Development core flow live during the workshop: constitution, specify, plan, tasks, and implement. Participants generate all SDD artifacts themselves, starting from a premise or statement that describes the exercise scenario; the repository provides reference code as a fallback, not pre-built SDD artifacts. Additionally, the flow MUST leverage optional Spec Kit steps when they add clarity or quality: clarify (to refine underspecified areas in the spec via targeted questions), analyze (to perform cross-artifact consistency and quality checks across spec, plan, and tasks), checklist (to generate a custom verification checklist for the current feature).

The customization-lab module (presets and extensions) MUST demonstrate how presets and extensions alter the Spec Kit experience by creating, installing, testing, and verifying customizations against the SDD flow. It exercises selected SDD commands (e.g., specify, plan) to validate customization effects, but is not required to complete the full core flow end-to-end.

Content-only modules (environment verification, introduction, closing) are exempt from the full SDD flow but SHOULD produce at least one visible and reviewable artifact in the repository to evidence learning.

Rationale: the full SDD flow applies where participants write and validate code from a scenario premise. The customization lab focuses on Spec Kit extensibility, not on repeating the end-to-end pipeline. Content-only modules serve pedagogical or logistical purposes and do not produce implementation artifacts, so mandating the full flow would be artificial. Retaining the artifact SHOULD ensures progress visibility across all modules.

### II. Dual Exercise Coverage (C# and Python - Non-Negotiable)

Every hands-on exercise MUST offer a functionally equivalent version in C# and Python, sharing the same functional objective, the same acceptance criteria, and comparable execution commands. No language may remain as an incomplete example.

Rationale: the audience is mixed and Spec Kit's value is best demonstrated when the methodology stays stable even as the technology stack changes.

### III. Executable and Reproducible Verification

Every workshop deliverable MUST include executable verification steps (build, test, and run) and a minimum expected output. Acceptance criteria MUST be verifiable with repeatable commands in under 10 minutes per exercise.

Rationale: an introductory workshop demands fast and objective feedback to reduce friction and ease in-room support.

### IV. Greenfield and Brownfield Adaptation

The content MUST demonstrate two scenarios within a 2-hour session:

1) Greenfield (new project).
2) Brownfield (existing project).

Each scenario MUST show how risks, decisions, and command sequences differ.

Rationale: Spec Kit adoption value depends on showing real usage in both new projects and systems already in production.

### V. Responsible Customization (Presets and Extensions)

Customization MUST prioritize simplicity and traceability: first local overrides, then presets, and finally extensions when new capabilities are required. Every customization MUST document its purpose, impact, and rollback procedure.

Rationale: prevents accidental complexity and makes the workshop easier to reproduce in other teaching environments.

### VI. Language Policy (Non-Negotiable)

All source code, SDD artifacts (constitution, spec, plan, tasks), commit messages, code comments, and technical documentation generated through the Spec-Driven Development flow MUST be written in English.

All workshop instructional materials intended for students, attendees, instructors, or facilitators (guides, slide decks, handouts, troubleshooting references, and `README.md` sections aimed at participants) MUST be written in Spanish and in Markdown.

This separation is non-negotiable: English ensures the codebase and SDD artifacts remain universally readable and tooling-compatible; Spanish ensures the learning experience is accessible to the target audience.

Rationale: a clear language boundary eliminates ambiguity about which artifacts use which language, prevents mixed-language drift, and respects both engineering best practices and the pedagogical needs of the workshop audience.

## Workshop Scope and Constraints (Non-Negotiable)

- Language policy: see Principle VI above; code and SDD artifacts in English, workshop materials for participants and instructors in Spanish and Markdown.
- Target duration: the instructional design MUST fit within around 2 hours, including introduction, exercises, troubleshooting, and closing.
- Technical sources: the technical source of truth MUST prioritize official documentation from github.com and microsoft.com; secondary sources may complement pedagogy.
- Minimum room requirements: Git, Python 3.11+, uv, the specify CLI, VS Code with GitHub Copilot, and the exercise language runtime (dotnet or python) MUST be verified at the start.
- Didactic approach: each block SHOULD end with a tangible deliverable in the repo to maintain pace and progress visibility.

## Operational Flow and Quality Gates

1. Preparation: validate the environment with diagnostic commands and leave evidence that the stack is ready.
2. Introduction: explain SDD and explicitly separate spec (what/why) from plan (how).
3. Greenfield: execute the full flow with a small, verifiable exercise.
4. Presets and Extensions: apply a minimal customization and verify its effect.
5. Brownfield: adapt the flow to an existing repository and deliver a feature.
6. Closing: record learnings, risks, and next steps.

Mandatory quality gates:

- Gate A (before coding): spec and plan free of blocking ambiguities.
- Gate B (before closing each exercise): minimum tests/execution in both C# and Python.
- Gate C (end of session): greenfield vs brownfield comparison with documented decisions and trade-offs.

## Governance

This constitution takes precedence over ad-hoc workshop guides. Every change proposal MUST include: rationale, impact on session time, impact on C#/Python exercises, and updates to affected templates.

Versioning policy:

- MAJOR: backward-incompatible changes to principles or removal of didactic pillars.
- MINOR: new principles, new quality gates, or material expansion of sections.
- PATCH: clarifications, wording, and non-semantic refinements.

Amendment and compliance process:

- Amendments MUST update this file and the Sync Impact Report in the same change.
- Every spec/plan/tasks review MUST include an explicit constitutional compliance check.
- If a rule does not apply in a specific session, the exception MUST be justified in writing in the corresponding plan artifact.

**Version**: 1.4.0 | **Ratified**: 2026-04-13 | **Last Amended**: 2026-04-13
