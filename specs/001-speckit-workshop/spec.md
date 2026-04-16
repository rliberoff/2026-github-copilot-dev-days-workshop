# Feature Specification: Spec Kit Introductory Workshop

**Feature Branch**: `001-speckit-workshop`
**Created**: 2026-04-13
**Status**: Draft
**Input**: User description: "Create specifications for the workshop following the constitution guidelines and the SPEC.md content."

## Clarifications

### Session 2026-04-13

- Q: What concrete artifact scope should the workshop implementation include for the dual-language exercises? → A: Deliver in-repo executable reference projects for C# and Python in both greenfield and brownfield exercises.
- Q: What verification scope should the custom extension command cover? → A: Provide two separate verification commands, one for .NET and one for Python.
- Q: Should the workshop use fixed canonical exercises or allow equivalents? → A: Fix the workshop to TodoLite for greenfield and Notes API for brownfield.
- Q: What is each participant expected to complete with respect to the dual-language paths? → A: Each participant completes one chosen language path, while the repository and instructor materials cover both.

## Workshop Constraints *(mandatory for this repository)*

These constraints are derived from the constitution and restated here for quick reference. The authoritative definitions live in the linked sources.

- **Timing**: ~2-hour introductory workshop (Constitution § Workshop Scope; FR-017).
- **Language policy**: Spanish Markdown for participant/instructor materials; English for code, SDD artifacts, and technical docs (Constitution § Principle VI; FR-012, FR-013).
- **Dual-language exercises**: Equivalent C# and Python paths (Constitution § Principle II; FR-004, FR-007).
- **Participant scope**: Each participant completes one chosen language path; the repository and instructor materials cover both C# and Python paths.
- **Greenfield and brownfield**: Both scenarios required (Constitution § Principle IV; FR-003, FR-006).
- **Reproducible verification**: Command-driven, under 10 minutes per exercise (Constitution § Principle III; FR-008).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Complete the greenfield SDD flow (Priority: P1)

A workshop participant with intermediate knowledge of C# or Python and Git wants to learn Spec-Driven Development by building a small project from scratch. The participant follows one chosen language path during the live workshop while the repository and instructor materials maintain equivalent C# and Python implementations. The participant follows the full SDD core flow (constitution, specify, plan, tasks, implement) using Spec Kit slash commands, producing visible artifacts at each step and a working application at the end. The workshop guide provides a premise describing the greenfield scenario; participants generate all SDD artifacts live during the session. Reference code in the repository serves as a fallback if the participant's agent-generated code diverges or time runs short.

**Why this priority**: The greenfield flow is the foundational learning experience. It demonstrates the entire SDD pipeline end-to-end in the simplest scenario. Without this, the other exercises lack context.

**Independent Test**: Can be fully tested by a participant executing the SDD flow from an empty repository and verifying that: all SDD artifacts exist, the application builds, tests pass, and the CLI/API responds correctly.

**Acceptance Scenarios**:

1. **Given** a participant with a verified environment (Git, Python 3.11+, uv, specify CLI, VS Code with GitHub Copilot, and language runtime installed), **When** the participant runs `specify init --here --ai copilot --force` in a new project, **Then** the `.specify/` directory is created with constitution, templates, and registered agent commands.
2. **Given** an initialized Spec Kit project, **When** the participant executes `/speckit.constitution` with workshop-provided principles, **Then** a `constitution.md` artifact is created and committed to the repository.
3. **Given** a project with a constitution, **When** the participant executes `/speckit.specify` describing the TodoLite application, **Then** a `spec.md` artifact is generated with functional requirements, acceptance criteria, and user scenarios.
4. **Given** a completed specification, **When** the participant optionally executes `/speckit.clarify`, **Then** underspecified areas are identified and resolved with targeted questions encoded back into the spec.
5. **Given** a completed specification, **When** the participant executes `/speckit.plan`, **Then** a `plan.md` artifact is generated with technical stack, design decisions, and a test plan.
6. **Given** a completed plan, **When** the participant executes `/speckit.tasks`, **Then** a `tasks.md` artifact is generated with an ordered, actionable task list derived from the plan.
7. **Given** generated tasks, **When** the participant optionally executes `/speckit.analyze`, **Then** a cross-artifact consistency check runs across spec, plan, and tasks, surfacing any misalignments.
8. **Given** a completed task list, **When** the participant executes `/speckit.implement` or follows the reference code, **Then** the application builds successfully, all tests pass, and the application produces the expected output.
9. **Given** a completed greenfield exercise, **When** the participant runs the validation commands (`dotnet test` / `pytest` and the application CLI/run command), **Then** the output matches the expected results documented in the exercise materials.

---

### User Story 2 - Customize Spec Kit with presets and extensions (Priority: P2)

A workshop participant wants to understand how to adapt Spec Kit to team or project needs. The participant creates a local preset that modifies spec and plan templates (adding verifiable acceptance criteria and a decisions table), and creates a local extension that adds two new slash commands for quick project verification, one for .NET and one for Python. This is a customization lab: participants create, install, test, and verify customizations against the SDD flow, exercising selected commands (e.g., specify, plan) to validate customization effects, but are not required to complete the full SDD core flow end-to-end.

**Why this priority**: Customization demonstrates Spec Kit's flexibility beyond the core flow and is essential for real-world adoption. It builds directly on the greenfield experience.

**Independent Test**: Can be fully tested by installing the preset and extension locally, verifying that template resolution reflects the changes, and executing both verification slash commands (dotnet.quickcheck against the C# sample project and python.quickcheck against the Python sample project) successfully.

**Acceptance Scenarios**:

1. **Given** a Spec Kit project from the greenfield exercise, **When** the participant creates a preset with modified `spec-template.md` and `plan-template.md` and installs it via `specify preset add --dev`, **Then** `specify preset list` shows the preset and `specify preset resolve spec-template` returns the overridden template.
2. **Given** an installed preset, **When** the participant creates a new specification using `/speckit.specify`, **Then** the generated spec follows the customized template structure (including the new sections from the preset).
3. **Given** a Spec Kit project, **When** the participant creates an extension with an `extension.yml` and a command Markdown file and installs it via `specify extension add --dev`, **Then** `specify extension list` shows the extension and `specify extension info <id>` displays its metadata.
4. **Given** an installed extension with separate .NET and Python verification commands (`dotnet.quickcheck` and `python.quickcheck`), **When** the participant executes `/dotnet.quickcheck` against the C# sample project and `/python.quickcheck` against the Python sample project, **Then** each command runs the defined verification steps (build + test for C#, install + pytest for Python) and produces a summary report.
5. **Given** both preset and extension installed, **When** the participant runs `specify preset resolve <template-name>`, **Then** the template resolution follows the documented priority stack: overrides, presets, extensions, core.

---

### User Story 3 - Apply SDD to an existing brownfield project (Priority: P3)

A workshop participant wants to adopt Spec Kit on a pre-existing repository with working code. The participant initializes Spec Kit in the repo, optionally uses brownfield-specific tooling to scan the codebase, and then defines and implements a new feature using the SDD flow on one chosen language path during the live workshop, while the repository and instructor materials maintain equivalent C# and Python implementations. The workshop guide provides a premise describing the brownfield scenario and the new feature to add; participants generate the SDD artifacts live during the session. Reference code in the repository serves as a fallback.

**Why this priority**: Brownfield adoption shows that SDD is not limited to new projects and addresses the most common real-world scenario: adding discipline to existing codebases.

**Independent Test**: Can be fully tested by starting with the pre-built Notes API baseline, initializing Spec Kit, generating SDD artifacts for a new feature, implementing it, and verifying the new endpoint works alongside existing functionality.

**Acceptance Scenarios**:

1. **Given** a participant with a pre-existing working project (Notes API baseline with build and run verified), **When** the participant runs `specify init --here --ai copilot --force`, **Then** Spec Kit initializes without disrupting existing code, and the `.specify/` directory is created.
2. **Given** an initialized brownfield Spec Kit project, **When** the participant executes `/speckit.constitution` with the workshop-provided principles, **Then** a `constitution.md` artifact is created for the brownfield flow without disrupting the existing Notes API baseline.
3. **Given** a brownfield project with a constitution, **When** the participant executes `/speckit.specify` describing a new feature (search endpoint), **Then** a spec is generated that accounts for the existing codebase context.
4. **Given** a brownfield specification or plan with unresolved ambiguity, validation gaps, or cross-artifact risk, **When** the participant optionally executes `/speckit.clarify`, `/speckit.checklist`, or `/speckit.analyze` at the appropriate point in the flow, **Then** the brownfield artifacts are refined, reviewed, or checked for consistency before implementation continues.
5. **Given** a brownfield spec and plan, **When** the participant executes `/speckit.tasks` and then implements the feature, **Then** the new feature works alongside existing endpoints without breaking them.
6. **Given** the implemented brownfield feature, **When** the participant runs validation commands (build, test, and endpoint verification), **Then** both the original endpoints and the new search endpoint return correct results.
7. **Given** a brownfield project, **When** the participant compares the greenfield and brownfield experiences, **Then** the participant can articulate at least two concrete differences in risks, decisions, or command sequences between the two scenarios.

### User Story 4 - Set up and verify the workshop environment (Priority: P4)

A workshop participant arrives at the session and needs to verify that all prerequisites are installed and working before exercises begin. The participant runs diagnostic commands and confirms readiness within the first 5 minutes.

**Why this priority**: Environment verification is a prerequisite for all exercises. Completing it quickly reduces friction and maximizes hands-on time.

**Independent Test**: Can be fully tested by running the verification checklist commands and confirming all tools report expected versions/status.

**Acceptance Scenarios**:

1. **Given** a participant workstation, **When** the participant runs `specify version`, **Then** the installed Spec Kit version is displayed.
2. **Given** a participant workstation, **When** the participant runs `specify check`, **Then** the tool reports the status of all prerequisites (Git, Python, VS Code with GitHub Copilot).
3. **Given** any prerequisite is missing or misconfigured, **When** the participant consults the troubleshooting reference, **Then** the reference provides a specific, actionable resolution for the detected issue.

---

### User Story 5 - Instructor delivers the workshop with structured guidance (Priority: P5)

An instructor facilitates the workshop using structured materials, troubleshooting guides, and timing references. The instructor introduces SDD concepts, guides exercises, handles participant issues, and conducts a closing retrospective.

**Why this priority**: Instructor materials ensure consistent delivery and quality across different facilitators and sessions.

**Independent Test**: Can be fully tested by an instructor conducting a dry run of the full workshop using only the provided materials and verifying that all exercises complete within the time allocation.

**Acceptance Scenarios**:

1. **Given** an instructor with the workshop materials, **When** the instructor follows the introduction section, **Then** participants can distinguish between spec (what/why) and plan (how) with a concrete example.
2. **Given** a participant encountering a common issue (command not found, commands not appearing in agent, branch errors), **When** the instructor consults the troubleshooting table, **Then** a specific diagnosis and resolution is available for the issue.
3. **Given** the closing section, **When** the instructor facilitates the retrospective, **Then** participants reflect on which SDD phase added the most clarity, how ambiguities were resolved, and what customizations their teams would need.

---

### Edge Cases

- What happens when `specify` is not in the participant's PATH after installation? The troubleshooting guide MUST provide resolution steps for PATH configuration on Windows, macOS, and Linux.
- What happens when the agent does not display Spec Kit slash commands after `specify init`? The materials MUST document agent-specific troubleshooting (restart agent, verify command directories, re-run init with `--force`).
- What happens when `uv` or Python 3.11+ is not installed? The materials MUST provide installation instructions or a pre-configured environment alternative.
- What happens when a participant's exercise code diverges from the reference solution? Reference code MUST be provided as a fallback that participants can copy and execute directly.
- What happens when a brownfield project scan takes too long? The materials MUST document context reduction strategies (ignoring heavy directories, splitting features).
- What happens when a preset or extension installation fails due to version incompatibility? The troubleshooting guide MUST explain version requirements and how to verify compatibility.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Workshop MUST include an environment verification module that validates shared prerequisites (Git, Python 3.11+, uv, the `specify` CLI, VS Code with GitHub Copilot) and the language runtime for the participant's chosen exercise path (`dotnet` for C# or `python` for Python). Participants are not required to validate both runtimes.
- **FR-002**: Workshop MUST include an introduction module that explains Spec-Driven Development, the separation between spec (what/why) and plan (how), and the Spec Kit core flow (constitution → specify → plan → tasks → implement).
- **FR-003**: Workshop MUST include a greenfield exercise where participants build the TodoLite application from scratch using the full SDD flow, producing SDD artifacts and working code.
- **FR-004**: Greenfield exercise MUST offer functionally equivalent implementations in C# and Python, sharing the same functional objective, acceptance criteria, and comparable execution commands.
- **FR-004A**: Each participant MUST be able to complete the greenfield exercise by following one chosen language path during the live workshop, while the repository and instructor materials provide equivalent C# and Python implementations.
- **FR-005**: Workshop MUST include a presets and extensions module where participants create, install, and verify a local preset and a local extension that provides separate .NET and Python verification commands.
- **FR-006**: Workshop MUST include a brownfield exercise where participants initialize Spec Kit in the pre-built Notes API repository and implement a new feature using the SDD flow.
- **FR-007**: Brownfield exercise MUST offer functionally equivalent implementations in C# and Python, sharing the same functional objective, acceptance criteria, and comparable execution commands.
- **FR-007A**: Each participant MUST be able to complete the brownfield exercise by following one chosen language path during the live workshop, while the repository and instructor materials provide equivalent C# and Python implementations.
- **FR-008**: Every exercise MUST include documented, executable verification steps appropriate to the exercise type — build, test, and run for code-producing exercises (greenfield, brownfield); install, resolve, and command execution for customization exercises (presets and extensions) — with documented expected output, verifiable in under 10 minutes per exercise.
- **FR-009**: Every hands-on exercise module (greenfield, brownfield, presets and extensions) MUST produce at least one visible and reviewable artifact in the repository. Content-only modules (environment verification, introduction, closing) SHOULD produce at least one visible artifact to evidence learning.
- **FR-010**: Workshop MUST surface optional Spec Kit steps (clarify, analyze, or checklist) in both SDD-flow exercises when they add clarity or quality, and MUST explicitly demonstrate at least one optional step during the greenfield exercise to showcase the full depth of the toolkit.
- **FR-011**: Workshop MUST include troubleshooting guidance covering: CLI not found, prerequisites missing, agent commands not appearing, branch/feature errors, and template/extension issues.
- **FR-012**: Workshop materials for participants and instructors (guides, handouts, troubleshooting references under `materials/`) MUST be written in Spanish and Markdown. `README.md` files inside exercise projects (e.g., `exercises/presets/`, `exercises/extensions/`) are technical project documentation and fall under FR-013, not FR-012.
- **FR-013**: All source code, SDD artifacts (constitution, spec, plan, tasks), commit messages, code comments, technical documentation, and project-level `README.md` files under `exercises/` and `specs/` MUST be written in English.
- **FR-014**: Workshop MUST include reference code for all exercises that participants can copy and execute directly if their agent-generated code diverges or time runs short.
- **FR-015**: Workshop MUST include a closing module with a structured retrospective comparing greenfield vs brownfield experiences and documenting decisions, trade-offs, and next steps.
- **FR-016**: Workshop MUST demonstrate the preset template resolution priority stack (overrides → presets → extensions → core) with a practical example.
- **FR-017**: Workshop timing MUST fit within approximately 2 hours, with each module having a defined time allocation.
- **FR-018**: Workshop repository MUST contain standalone executable reference projects for both C# and Python for the greenfield and brownfield exercises; inline snippets alone are insufficient.

### Key Entities

- **Workshop Module**: A discrete block of the workshop with a defined objective, time allocation, and tangible deliverable. Modules include: environment verification, introduction, greenfield exercise, presets and extensions, brownfield exercise, and closing.
- **Exercise**: A hands-on coding activity within a module. Each exercise has a functional objective, dual-language implementations (C# and Python), standalone executable reference projects in the repository, acceptance criteria, reference code, and verification commands.
- **SDD Artifact**: A Spec Kit output document (constitution, spec, plan, tasks) generated during the SDD flow. Each artifact is committed to the repository and serves as evidence of learning.
- **Preset**: A Spec Kit customization package that overrides templates and/or commands without modifying core functionality. Defined by a `preset.yml` manifest.
- **Extension**: A Spec Kit capability package that adds new commands, hooks, or integrations. Defined by an `extension.yml` manifest.
- **Troubleshooting Entry**: A mapping of symptom, probable cause, and resolution for a known workshop issue.

## Success Criteria *(mandatory)*

### Implementation Acceptance Criteria

- **SC-006**: Each exercise provides functionally equivalent paths in both C# and Python, with matching acceptance criteria and comparable verification commands.
- **SC-007**: Participants can articulate the difference between spec (what/why) and plan (how) after the introduction module.
- **SC-008**: Participants can identify at least two differences between greenfield and brownfield SDD adoption after completing both exercises.

### Workshop Delivery Targets (Non-Gating)

The following criteria are design targets for live workshop delivery. They guide time allocation and material quality but do not gate implementation tasks. They are included for traceability and instructor dry-run planning.

- **SC-001** *(target for FR-001)*: 100% of participants who complete the environment verification can execute `specify version` and `specify check` successfully within 5 minutes.
- **SC-002** *(target for FR-003)*: Participants completing the greenfield exercise produce at least 4 SDD artifacts (constitution, spec, plan, tasks) and a working application in under 45 minutes.
- **SC-003** *(target for FR-005)*: Participants completing the presets and extensions module can install, list, and verify a custom preset and extension in under 25 minutes.
- **SC-004** *(target for FR-006)*: Participants completing the brownfield exercise can initialize Spec Kit in an existing project and deliver a new working feature in under 25 minutes.
- **SC-005** *(target for FR-008)*: On at least 90% of participant machines, the verification commands defined in FR-008 execute with zero failures and their output matches the documented expected results.
- **SC-009** *(target for FR-017)*: Instructor dry-run completes all modules within the time allocations defined in FR-017, with total duration between 110 and 130 minutes.
- **SC-010** *(target for FR-011)*: Every common issue documented in the troubleshooting guide has a specific, actionable resolution that can be applied by a participant in under 3 minutes.

### Validation Commands *(mandatory)*

- **CSharp Build/Test/Run (Greenfield)**: `dotnet build`, `dotnet test`, `dotnet run --project src/TodoLite.Cli -- add "Buy milk"`, `dotnet run --project src/TodoLite.Cli -- list`
- **Python Run/Test (Greenfield)**: `pip install -e ".[dev]"`, `pytest`, `python -m todolite add "Buy milk"`, `python -m todolite list`
- **CSharp Build/Test/Run (Brownfield)**: `dotnet build`, `dotnet test`, `dotnet run --project src/Notes.Api`, followed by HTTP requests to `/notes`, `/notes/search?q=...&tag=...`
- **Python Run/Test (Brownfield)**: `pip install -e ".[dev]"`, `pytest`, `python -m notes_api`, followed by HTTP requests to `/notes`, `/notes/search?q=...&tag=...`
- **Spec Kit Verification**: `specify version`, `specify check`, `specify preset list`, `specify extension list`
- **Presets/Extensions Verification (US2)**:
  - Install preset: `specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5` → expected: exit 0, preset appears in `specify preset list`
  - Resolve preset template: `specify preset resolve spec-template` → expected: output contains "Verifiable Acceptance Criteria" section
  - Install extension: `specify extension add --dev exercises/extensions/my-ext` → expected: exit 0, extension appears in `specify extension list`
  - C# quickcheck: `/dotnet.quickcheck` against exercises/extensions/my-ext/sample/csharp/ → expected: `dotnet build` succeeds, `dotnet test` reports 1 passed / 0 failed, summary shows PASS
  - Python quickcheck: `/python.quickcheck` against exercises/extensions/my-ext/sample/python/ → expected: `pytest` reports 1 passed / 0 failed, summary shows PASS
  - Priority stack: `specify preset resolve spec-template` with override installed → expected: output contains "Project Banner" (overrides layer wins)
  - Rollback preset: `specify preset remove dotnet-workshop-lite` → expected: `specify preset list` no longer shows preset
  - Rollback extension: `specify extension remove my-ext` → expected: `specify extension list` no longer shows extension
- **Expected Evidence**: SDD artifacts in `specs/` directory, runnable C# and Python reference projects for both exercises in the repository, passing tests with zero failures, and correct CLI/API output matching documented examples

## Assumptions

- Participants have intermediate knowledge of at least one of the exercise languages (C# or Python) and basic Git proficiency (clone, commit, push, branch).
- All participants use VS Code with GitHub Copilot as their editor and AI agent for all exercises, regardless of the chosen language path.
- Participants choose either the C# or Python path for each exercise during the live workshop; they are not required to complete both languages because parity is preserved by the repository and instructor materials.
- An internet connection is available for installing `specify-cli`, downloading extensions, and accessing documentation. Offline alternatives exist but are not the primary path.
- The workshop environment has been pre-validated by the instructor using the environment verification checklist before participants arrive.
- Spec Kit CLI version 0.6.x or later is used, matching the features documented in the official repository.
- The greenfield exercise is fixed to TodoLite in both C# and Python, with matching CLI behavior, persistence format, and validation commands.
- The brownfield exercise is fixed to Notes API in both C# and Python, with matching endpoint behavior, payload shapes, and validation commands.
- The preset example focuses on modifying spec and plan templates (adding verifiable acceptance criteria and a decisions table), not on creating complex multi-template packages.
- The extension example focuses on adding two simple verification commands, one for .NET and one for Python, not on complex multi-command integrations.
- The Brownfield Bootstrap extension (`spec-kit-brownfield`) is demonstrated as an optional enhancement, not a mandatory dependency for the brownfield exercise.
- Standard web/API performance expectations apply; no specific load or concurrency targets are defined for the exercise applications.
