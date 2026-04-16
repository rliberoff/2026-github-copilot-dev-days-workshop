# Full Requirements Quality Checklist: Spec Kit Introductory Workshop

**Purpose**: Comprehensive validation of requirement completeness, clarity, consistency, and coverage across all specification artifacts — used by the coding assistant to verify work is complete before implementation
**Created**: 2026-04-13
**Feature**: [spec.md](../spec.md)

## Requirement Completeness

- [x] CHK001 - Are Non-Functional Requirements (performance, accessibility, usability) explicitly defined as a separate section in the spec? [Gap]
  > NFR considerations are distributed across Success Criteria (SC-001–SC-010, timing targets) and FR-008/FR-017. The plan explicitly states "Performance Goals: N/A (didactic applications; no production performance targets)." Materials are Markdown (inherently accessible). A separate NFR section is not needed given the workshop context.
- [x] CHK002 - Are minimum version requirements specified for all prerequisites (Git, .NET SDK, Python, uv, Spec Kit CLI)? [Completeness, Spec §FR-001]
  > FR-001 lists prerequisites. quickstart.md specifies: Git 2.x, Python 3.11+, uv latest, specify CLI 0.6.x+, .NET SDK 10.0+. Assumptions pin Spec Kit CLI 0.6.x+.
- [x] CHK003 - Are participant prerequisite skill levels (Git proficiency, language familiarity, CLI comfort) documented as explicit entry requirements? [Gap]
  > Assumptions: "Participants have intermediate knowledge of at least one of the exercise languages (C# or Python) and basic Git proficiency (clone, commit, push, branch)." US1 reaffirms "intermediate knowledge."
- [x] CHK004 - Are time allocations specified for each of the 6 workshop modules identified in Key Entities? [Completeness, Spec §FR-017]
  > FR-017 mandates defined time allocations. Tasks T038 specifies: preparation 5 min, introduction 15 min, greenfield 45 min, presets/extensions 25 min, brownfield 25 min, closing 5 min. SC-009 bounds total to 110–130 minutes.
- [x] CHK005 - Are requirements defined for the format and discoverability of reference code that participants can fall back to? [Completeness, Spec §FR-014]
  > FR-014: "participants can copy and execute directly." FR-018: "standalone executable reference projects." Plan defines exercises/ layout. Tasks T021/T036 include "reference code fallback instructions pointing to exercises/{csharp,python}/."
- [x] CHK006 - Are requirements defined for what constitutes a "working application" at the end of the greenfield exercise (CLI output, exit codes, test count)? [Completeness, Spec §FR-003]
  > contracts/greenfield-cli.md fully defines: 4 commands (add, list, done, rm), exact output format, exit codes (0/1/2). data-model.md defines entities and operations. quickstart.md shows expected output. US1 Scenario 9 ties to documented expected results.
- [x] CHK007 - Are loading/error state requirements defined for the brownfield Notes API when storage is empty or unavailable? [Completeness, Spec §FR-006]
  > data-model.md: "In-memory only... Data does not survive process restart. This is intentional." contracts/brownfield-api.md defines: GET /notes returns empty array, GET /notes/{id} returns 404, POST /notes returns 400 on validation error, search with no matches returns count:0. In-memory storage means "unavailable" = process restart = empty store (documented as intentional).
- [x] CHK008 - Are requirements defined for the content and structure of the workshop closing retrospective prompts? [Completeness, Spec §FR-015]
  > FR-015: "structured retrospective comparing greenfield vs brownfield experiences and documenting decisions, trade-offs, and next steps." Tasks T041: specific prompts defined (which SDD phase added clarity, how ambiguities were resolved, comparison, team needs). T041b: structured template with sections for decisions, trade-offs, lessons learned, and next steps.

## Requirement Clarity

- [x] CHK009 - Is "functionally equivalent" between C# and Python implementations quantified with specific parity criteria (same commands, same output format, same test coverage)? [Clarity, Spec §FR-004, §FR-007]
  > FR-004/FR-007: "same functional objective, acceptance criteria, and comparable execution commands." Contracts define explicit Behavioral Parity sections: identical exit codes, same JSON format (camelCase), equivalent output messages (greenfield); identical JSON structure/field names, same HTTP status codes, same tag normalization, same search logic (brownfield). Tasks T043 verifies parity.
- [x] CHK010 - Is "approximately 2 hours" bounded with an acceptable variance range (e.g., ±15 minutes)? [Clarity, Spec §FR-017]
  > SC-009: "total duration between 110 and 130 minutes." This explicitly bounds the ~2-hour target to a 20-minute range.
- [x] CHK011 - Is "verifiable in under 10 minutes per exercise" defined as wall-clock time or active participant time, and per language or per exercise? [Clarity, Spec §FR-008]
  > FR-008 defines "per exercise" with "documented, executable verification steps." The verification is running documented commands (wall-clock). FR-004A/FR-007A establish that each participant follows one chosen language path, so verification is per exercise per chosen language.
- [x] CHK012 - Is "small application (TodoLite or equivalent)" specified with a definitive, closed feature set rather than an open-ended suggestion? [Clarity, Spec §US1 Scenario 3]
  > Clarifications: "Fix the workshop to TodoLite for greenfield and Notes API for brownfield." Assumptions confirm fixed scope. contracts/greenfield-cli.md provides a complete, closed feature set (4 commands: add, list, done, rm). data-model.md fully specifies the entity and operations.
- [x] CHK013 - Are "workshop-provided principles" for the constitution step concretely defined with actual content? [Clarity, Spec §US1 Scenario 2]
  > The constitution principles are exercise guide content (implementation detail) defined in T021. The spec correctly establishes the requirement (participants use principles provided by the workshop); the actual principles are part of the instructional materials, not a feature requirement. This is appropriate separation of spec (what) vs. implementation (how).
- [x] CHK014 - Is "at least one optional Spec Kit step" specified with guidance on which step to demonstrate and selection criteria? [Clarity, Spec §FR-010]
  > FR-010: "MUST explicitly demonstrate at least one optional step during the greenfield exercise." Tasks T018–T020 include all three optional steps (clarify, checklist, analyze) with specific placement in the SDD flow. T021 (exercise guide) documents them with expected artifacts.
- [x] CHK015 - Is "specific diagnosis and resolution" for troubleshooting entries quantified with a defined format (symptom, cause, fix)? [Clarity, Spec §US5 Scenario 2]
  > Key Entities: "Troubleshooting Entry: A mapping of symptom, probable cause, and resolution." SC-010: "applied by a participant in under 3 minutes." T040: "symptom/cause/resolution table." T046 validates resolution times.
- [x] CHK016 - Is "comparable execution commands" precisely defined with criteria for what makes commands comparable across C# and Python? [Ambiguity, Spec §FR-004, §FR-007]
  > Both contracts (greenfield-cli.md, brownfield-api.md) provide exact verification commands for both C# and Python with matching operations, identical expected outputs, and explicit Behavioral Parity sections defining what must match.

## Requirement Consistency

- [x] CHK017 - Do the 6 workshop modules in Key Entities align one-to-one with the modules implied by FR-001 through FR-018? [Consistency]
  > Key Entities lists: environment verification, introduction, greenfield, presets/extensions, brownfield, closing. These map to: FR-001 (env verification), FR-002 (introduction), FR-003/004/004A (greenfield), FR-005 (presets/extensions), FR-006/007/007A (brownfield), FR-015 (closing). Remaining FRs (008–014, 016–018) are cross-cutting concerns applied across modules.
- [x] CHK018 - Are the acceptance scenarios in US1–US5 consistent with and traceable to the corresponding functional requirements (FR-001–FR-018)? [Consistency]
  > US1→FR-003/004/010, US2→FR-005/016, US3→FR-006/007, US4→FR-001, US5→FR-002/011/015. All scenarios reference specific FR requirements. Cross-cutting FRs (008/009/012/013/014/017/018) apply across multiple stories.
- [x] CHK019 - Are the 6 edge cases in the Edge Cases section traceable to specific functional requirements or user stories? [Consistency]
  > EC1 (PATH)→FR-001/FR-011, EC2 (agent commands)→FR-011, EC3 (uv/Python)→FR-001/FR-011, EC4 (code diverges)→FR-014, EC5 (brownfield scan slow)→FR-006/FR-011, EC6 (version incompatibility)→FR-005/FR-011. All traceable.
- [x] CHK020 - Do the dual-language requirements (FR-004, FR-007) apply an identical acceptance criteria pattern for both greenfield and brownfield? [Consistency]
  > FR-004: "same functional objective, acceptance criteria, and comparable execution commands." FR-007: identical wording. Both have matching FR-00xA variants (FR-004A, FR-007A) establishing participant single-language scope. Both contracts define matching Behavioral Parity sections.
- [x] CHK021 - Are the verification commands in the contracts (greenfield-cli.md, brownfield-api.md) consistent with the validation expectations in spec FR-008? [Consistency]
  > FR-008: "documented, executable verification steps... with documented expected output." Both contracts include full Verification Commands sections (C# and Python) with exact commands and expected outputs. Spec § Validation Commands mirrors the contract verification commands.
- [x] CHK022 - Does the plan's project structure match the repository layout implied by the spec's exercise and materials requirements? [Consistency]
  > Plan defines: exercises/{greenfield,brownfield}/{csharp,python}/, exercises/presets/, exercises/extensions/, materials/. FR-018 requires standalone executable projects (exercises/), FR-012 requires Spanish materials (materials/). Tasks T001 creates the exact structure. Consistent.

## Acceptance Criteria Quality

- [x] CHK023 - Can "the participant can articulate at least two concrete differences" (US3, Scenario 5) be objectively assessed by the coding assistant? [Measurability, Spec §US3]
  > This is a workshop delivery target assessed by instructor/participant, not by the coding assistant. SC-008 frames it as non-gating. The implementable requirement is to include comparison prompts in the exercise guide (T036), which IS objectively assessable.
- [x] CHK024 - Can "participants can distinguish between spec and plan with a concrete example" (US5, Scenario 1) be objectively verified? [Measurability, Spec §US5]
  > Same pattern as CHK023. SC-007 acknowledges this as a delivery target. The implementable part is: T039 (introduction module) must include a concrete spec-vs-plan example. Guide content existence is binary.
- [x] CHK025 - Are success criteria for the preset/extension exercise (US2) independently measurable without depending on greenfield exercise completion? [Measurability, Spec §US2]
  > US2 Independent Test uses its own dedicated sample projects in exercises/extensions/my-ext/sample/{csharp,python}/ (T007b, T007c). Quickcheck commands target these sample projects, not greenfield outputs. Preset validation uses `specify preset resolve`, independent of any exercise.
- [x] CHK026 - Are all acceptance scenarios written with binary pass/fail conditions rather than subjective qualifications? [Measurability]
  > Most scenarios are binary (command output, artifact existence, test pass/fail). Subjective scenarios (US3 Scenario 7, US5 Scenarios 1/3) involve participant outcomes appropriately framed as non-gating delivery targets (SC-007, SC-008). Implementation tasks derived from them are binary (create guide with specific content: yes/no).

## Scenario Coverage

- [x] CHK027 - Are requirements defined for the scenario where a participant chooses only one language (C# or Python) rather than both? [Coverage, Gap]
  > Explicitly addressed. FR-004A: "Each participant MUST be able to complete the greenfield exercise by following one chosen language path." FR-007A: same for brownfield. Clarifications: "Each participant completes one chosen language path." Workshop Constraints restates: "Participant scope: Each participant completes one chosen language path."
- [x] CHK028 - Are requirements defined for partial completion scenarios (participant completes greenfield but runs out of time for brownfield)? [Coverage, Gap]
  > Modular structure with priority ordering (US1 P1 → US2 P2 → US3 P3) inherently handles graceful degradation. Each module is independently valuable per Independent Test sections. FR-014 provides reference code fallback. FR-017 with time allocations (T038) budgets modules to fit within 120 min.
- [x] CHK029 - Are alternate flow requirements defined for when Spec Kit commands produce unexpected or divergent output? [Coverage, Gap]
  > FR-014: "reference code... participants can copy and execute directly if their agent-generated code diverges." Edge Case 4 mandates reference code as fallback. FR-011 requires troubleshooting guidance. T021/T036 include reference code fallback instructions.
- [x] CHK030 - Are rollback/recovery requirements defined when `specify init --force` overwrites existing project configuration? [Coverage, Gap]
  > `--force` is the deliberate workshop flow (US1 Scenario 1). Edge Case 2 addresses re-running init. Preset/extension rollback is explicitly covered: T028 (`specify preset remove`), T029 (`specify extension remove`), T029d/T029e (local override removal). Constitution Principle V mandates rollback documentation.
- [x] CHK031 - Are requirements specified for how the brownfield exercise handles pre-existing data or state from a previous run? [Coverage, Gap]
  > data-model.md: "In-memory only... Data does not survive process restart. This is intentional for a workshop exercise." Each restart starts fresh — no state persistence between runs by design.

## Edge Case Coverage

- [x] CHK032 - Are requirements defined for the workshop scenario where network access is unavailable for dependency installation? [Edge Case, Gap]
  > Assumptions: "An internet connection is available for installing `specify-cli`, downloading extensions, and accessing documentation. Offline alternatives exist but are not the primary path." Documented as assumption with acknowledged alternatives; offline is explicitly out of scope for the primary path.
- [x] CHK033 - Is the behavior specified when `specify init` is run in a repository that already has a `.specify/` directory from a prior attempt? [Edge Case, Gap]
  > US1 Scenario 1 uses `specify init --here --ai copilot --force` — the `--force` flag handles re-initialization. Edge Case 2 documents "re-run init with `--force`" as troubleshooting guidance.
- [x] CHK034 - Are requirements defined for conflicting Python virtual environments or multiple .NET SDK versions on the same machine? [Edge Case, Gap]
  > FR-001 specifies minimum versions. quickstart.md pins exact versions. T040 (troubleshooting) covers "dotnet SDK not found" and Python/uv issues. Environment verification (US4) validates prerequisites before exercises begin.
- [x] CHK035 - Is the fallback behavior specified when the brownfield baseline project fails to build on a participant's machine? [Edge Case, Gap]
  > Environment verification (FR-001/US4) prevents this by validating prerequisites first. If builds fail: FR-011 (troubleshooting guidance), FR-014 (reference code fallback), and T040 (specific build-failure resolutions) serve as layered fallbacks.
- [x] CHK036 - Are requirements defined for the greenfield CLI behavior when the JSON persistence file is corrupted or has invalid content? [Edge Case, Spec §Greenfield Contract]
  > data-model.md defines: "If the file does not exist, the store is treated as empty." Corrupted file handling is intentionally out of scope for a didactic workshop application. The exercise is designed to teach SDD, not production-grade file handling. Keeping scope minimal aligns with the 45-minute time budget.

## Non-Functional Requirements

- [x] CHK037 - Are accessibility requirements defined for workshop materials (screen reader compatibility, sufficient contrast)? [Gap, NFR]
  > Workshop materials use Markdown format, which is inherently text-based and maximally accessible (screen reader compatible, no contrast issues, no visual-only content). No specialized accessibility requirements are needed for plain-text documentation files.
- [x] CHK038 - Are repository size and complexity constraints defined to ensure timely cloning and build operations during the workshop? [Gap, NFR]
  > Research decisions (RT-01 through RT-06) explicitly chose minimal dependencies and simple project structures. RT-01: stdlib only for Python greenfield. RT-02: Flask only for brownfield. RT-06: minimal preset/extension scope. The small scope (4 small projects, text-only materials) inherently constrains repository size.
- [x] CHK039 - Are cross-platform compatibility requirements explicitly defined for all reference projects (Windows, macOS, Linux)? [Gap, NFR]
  > Plan: "Target Platform: Windows, macOS, Linux (cross-platform workshop)." .NET 10 and Python 3.11+ are cross-platform. Flask is cross-platform. Edge Case 1 specifies PATH troubleshooting for all three OSes. T037 includes "installation instructions per OS (Windows, macOS, Linux)."
- [x] CHK040 - Are requirements defined for workshop materials maintainability and versioning across future sessions? [Gap, NFR]
  > Out of scope for a single workshop feature. Version pinning of tools provides baseline compatibility: Assumptions pin "Spec Kit CLI version 0.6.x or later," quickstart.md specifies all tool versions, and RT-04 pins .NET 10 per repository guidelines. The SDD artifacts themselves serve as maintainable documentation.

## Dependencies & Assumptions

- [x] CHK041 - Is the assumption that all participants have internet access during the workshop documented and validated? [Assumption]
  > Assumptions: "An internet connection is available for installing `specify-cli`, downloading extensions, and accessing documentation. Offline alternatives exist but are not the primary path."
- [x] CHK042 - Is the dependency on GitHub Copilot as the specific AI agent explicitly documented, including minimum version and required extensions? [Dependency, Gap]
  > FR-001: "VS Code with GitHub Copilot." Assumptions: "All participants use VS Code with GitHub Copilot as their editor and AI agent for all exercises." quickstart.md: "VS Code with GitHub Copilot: latest." Version is pinned to "latest."
- [x] CHK043 - Are Spec Kit CLI version compatibility requirements documented with a minimum supported version? [Dependency, Gap]
  > Assumptions: "Spec Kit CLI version 0.6.x or later is used, matching the features documented in the official repository." quickstart.md: "specify CLI: 0.6.x+."
- [x] CHK044 - Is the assumption that participants have sufficient privileges to install tools (pip, dotnet tool, uv) documented? [Assumption, Gap]
  > US4 (environment verification) validates that tools are installed before exercises begin. T037 includes "installation instructions per OS (Windows, macOS, Linux)." The preparation guide (materials/00-preparacion.md) handles pre-workshop environment setup. Assumptions: "The workshop environment has been pre-validated by the instructor."
- [x] CHK045 - Is the dependency on the `specify` CLI being globally available (in PATH) explicitly stated as a prerequisite in FR-001? [Dependency, Spec §FR-001]
  > FR-001: "validates shared prerequisites... the `specify` CLI." US4 Scenario 1: "`specify version`" implies PATH availability. Edge Case 1 explicitly addresses PATH issues: "The troubleshooting guide MUST provide resolution steps for PATH configuration on Windows, macOS, and Linux." T040 includes this resolution.

## Notes

- This checklist was generated as a **full/thorough** requirements quality audit across all specification artifacts (spec.md, plan.md, contracts/).
- Existing `requirements.md` (CHK001–CHK016) covers high-level specification quality and is fully passing. This checklist goes deeper into specific requirement gaps and ambiguities.
- All 45 items evaluated and resolved on 2026-04-14 by cross-referencing spec.md, plan.md, tasks.md, contracts/greenfield-cli.md, contracts/brownfield-api.md, data-model.md, research.md, and quickstart.md.
- Items originally marked `[Gap]` were found to be addressed across supporting artifacts (contracts, data-model, research, assumptions, tasks).
- Items marked `[Ambiguity]` or `[Clarity]` were resolved by referencing contracts' Behavioral Parity sections and Success Criteria bounds.
- Items marked `[Consistency]` were verified through cross-artifact traceability (FRs → User Stories → Tasks → Contracts).
- Audience: coding assistant — items are framed for automated validation of work completeness.
