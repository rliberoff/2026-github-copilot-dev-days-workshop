# Tasks: Spec Kit Introductory Workshop

**Input**: Design documents from `/specs/001-speckit-workshop/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Included — the spec (FR-008) requires documented, executable verification steps appropriate to each exercise type (build/test/run for code-producing exercises; install/resolve/quickcheck for customization exercises). Reference projects include test suites.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions
- Include language marker when relevant: `[C#]`, `[PY]`, or `[C#/PY]`

## Path Conventions

- **Exercises**: `exercises/{greenfield,brownfield}/{csharp,python}/`
- **Presets**: `exercises/presets/dotnet-workshop-lite-preset/`
- **Extensions**: `exercises/extensions/my-ext/`
- **Materials**: `materials/` (Spanish Markdown)

## Constitution Alignment

- [x] Every user story maps to executable validation steps (quickstart.md, contracts/).
- [x] Tasks cover both C# and Python implementations for greenfield (US1) and brownfield (US3).
- [x] Each story is identified as SDD-flow exercise (US1 greenfield, US3 brownfield), customization lab (US2 presets/extensions), or content-only (US4, US5).
- [x] SDD-flow exercises (US1, US3): guides instruct participants to perform the full core flow live from a scenario premise; reference code serves as fallback, not pre-built SDD artifacts.
- [x] Customization lab (US2): exercises selected SDD commands (specify, plan) to validate customization effects; not required to complete the full core flow end-to-end.
- [x] Verification evidence capture included (quickstart validation in Polish phase).
- [x] Preset and extension include install/verify/rollback tasks (US2, T022–T030) per Constitution Principle V.
- [x] Extension provides dual-language verification commands (dotnet.quickcheck and python.quickcheck) with matching C# and Python sample projects (T007b, T007c, T027, T027b, T029c, T029f) per Constitution Principle II and FR-005.
- [x] Optional Spec Kit steps (clarify, checklist, analyze) are demonstrated in greenfield flow (US1, T018–T020) and surfaced in brownfield flow guidance when useful (US3, T036) per FR-010 and Constitution Principle I.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create all project scaffolding, solution files, and package structures so that user story implementation can begin immediately.

- [x] T001 Create top-level directory structure per plan.md: exercises/greenfield/csharp/, exercises/greenfield/python/, exercises/brownfield/csharp/, exercises/brownfield/python/, exercises/presets/dotnet-workshop-lite-preset/, exercises/extensions/my-ext/, materials/
- [x] T002 [P] Create greenfield C# solution: TodoLite.sln, src/TodoLite.Core/TodoLite.Core.csproj (.NET 10, C# 14), src/TodoLite.Cli/TodoLite.Cli.csproj (console app referencing Core), test/TodoLite.Core.Tests/TodoLite.Core.Tests.csproj (xUnit, Microsoft.NET.Test.Sdk) in exercises/greenfield/csharp/ — include minimal compilable entry points: empty Program.cs in TodoLite.Cli and a placeholder test class in TodoLite.Core.Tests so that `dotnet build` succeeds immediately
- [x] T003 [P] Create greenfield Python package: pyproject.toml (name=todolite, python>=3.11, dev dependency pytest), todolite/\_\_init\_\_.py, todolite/\_\_main\_\_.py, todolite/models.py, todolite/repository.py, tests/\_\_init\_\_.py, tests/test_todo_app.py in exercises/greenfield/python/
- [x] T004 [P] Create brownfield C# solution: NotesApi.sln, src/Notes.Api/Notes.Api.csproj (.NET 10, C# 14, ASP.NET Minimal API), test/Notes.Api.Tests/Notes.Api.Tests.csproj (xUnit, Microsoft.NET.Test.Sdk, referencing Notes.Api) in exercises/brownfield/csharp/ — include minimal compilable entry points: empty Program.cs in Notes.Api and a placeholder test class in Notes.Api.Tests so that `dotnet build` succeeds immediately
- [x] T005 [P] Create brownfield Python package: pyproject.toml (name=notes-api, python>=3.11, dependency Flask>=3.0, dev dependency pytest), notes_api/\_\_init\_\_.py, notes_api/\_\_main\_\_.py, notes_api/app.py, tests/\_\_init\_\_.py, tests/test_notes_api.py in exercises/brownfield/python/
- [x] T006 [P] Create preset directory scaffold: templates/, commands/ subdirectories in exercises/presets/dotnet-workshop-lite-preset/
- [x] T007 [P] Create extension directory scaffold: commands/ subdirectory and sample/csharp/ and sample/python/ subdirectories in exercises/extensions/my-ext/
- [x] T007b [P] [US2] [C#] Create minimal sample C# project in exercises/extensions/my-ext/sample/csharp/: Sample.sln, src/Sample/Sample.csproj (.NET 10, C# 14, single Greeter class with Greet method), test/Sample.Tests/Sample.Tests.csproj (xUnit, one passing test for Greeter.Greet) — this project is owned by US2 and used as the target for the dotnet.quickcheck extension command
- [x] T007c [P] [US2] [PY] Create minimal sample Python project in exercises/extensions/my-ext/sample/python/: pyproject.toml (name=sample-greeter, python>=3.11, dev dependency pytest), sample_greeter/\_\_init\_\_.py, sample_greeter/greeter.py (single Greeter class with greet method), tests/\_\_init\_\_.py, tests/test_greeter.py (one passing test for Greeter.greet) — this project is owned by US2 and used as the target for the python.quickcheck extension command

**Checkpoint**: All project skeletons exist. `dotnet build` succeeds for all C# solutions (greenfield, brownfield, C# sample) and `pip install -e .` succeeds for all Python packages (greenfield, brownfield, Python sample).

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: No cross-story foundational blocking prerequisites for this feature. Each exercise is a self-contained, independently buildable project in its own directory. User story implementation can begin immediately after Phase 1.

**Checkpoint**: Foundation ready — user story implementation can now begin in parallel.

---

## Phase 3: User Story 1 — Complete the greenfield SDD flow (Priority: P1) 🎯 MVP

**Goal**: Deliver working TodoLite CLI reference projects in C# and Python, with tests and a Spanish exercise guide. Participants build a from-scratch application using the full SDD core flow.

**Independent Test**: Run `dotnet build && dotnet test` in exercises/greenfield/csharp/ and `pytest` in exercises/greenfield/python/. Execute all CLI commands (add, list, done, rm) and verify output matches the greenfield-cli.md contract.

**Applicability**: Greenfield

### C# Implementation

- [x] T008 [P] [US1] [C#] Implement TodoItem record (Id, Text, IsDone, CreatedAtUtc, DoneAtUtc) per data-model.md in exercises/greenfield/csharp/src/TodoLite.Core/TodoItem.cs
- [x] T009 [P] [US1] [C#] Implement JsonFileTodoRepository (load/save JSON array, auto-increment ID, file path configurable via --file flag, default todolite.json) in exercises/greenfield/csharp/src/TodoLite.Core/JsonFileTodoRepository.cs
- [x] T010 [US1] [C#] Implement TodoApp service (Add, List with --open filter, MarkDone idempotent, Remove) using JsonFileTodoRepository per contracts/greenfield-cli.md in exercises/greenfield/csharp/src/TodoLite.Core/TodoApp.cs
- [x] T011 [P] [US1] [C#] Implement CLI entry point with argparse-style command routing (add, list, done, rm, help) and exit codes (0 success, 1 not found, 2 usage error) per contracts/greenfield-cli.md in exercises/greenfield/csharp/src/TodoLite.Cli/Program.cs
- [x] T012 [P] [US1] [C#] Implement TodoApp unit tests: add creates task, list returns all, list --open filters done, done marks complete idempotently, rm removes task, not-found returns false, empty text rejected — in exercises/greenfield/csharp/test/TodoLite.Core.Tests/TodoAppTests.cs

### Python Implementation

- [x] T013 [P] [US1] [PY] Implement TodoItem dataclass (id: int, text: str, is_done: bool, created_at_utc: datetime, done_at_utc: datetime | None) per data-model.md in exercises/greenfield/python/todolite/models.py
- [x] T014 [P] [US1] [PY] Implement JsonFileTodoRepository (load/save JSON array, auto-increment ID, default todolite.json, pathlib-based) in exercises/greenfield/python/todolite/repository.py
- [x] T015 [US1] [PY] Implement TodoApp facade (add, list with open filter, mark_done idempotent, remove) using repository per contracts/greenfield-cli.md in exercises/greenfield/python/todolite/\_\_init\_\_.py
- [x] T016 [P] [US1] [PY] Implement CLI entry point with argparse (add, list --open, done, rm, help) and sys.exit codes (0 success, 1 not found, 2 usage error) per contracts/greenfield-cli.md in exercises/greenfield/python/todolite/\_\_main\_\_.py
- [x] T017 [P] [US1] [PY] Implement todo_app unit tests: add creates task, list returns all, list --open filters, done marks complete, rm removes, not-found returns false, empty text rejected — in exercises/greenfield/python/tests/test_todo_app.py

### Optional SDD Steps (FR-010)

> **Precondition**: T018–T020 are participant-driven workshop activities (not code implementation tasks). They operate on the SDD artifacts that participants generate live during the greenfield exercise (seeded by the scenario premise in the exercise guide, e.g. "Build a minimal TODO CLI called TodoLite…"). The resulting feature directory and its spec.md, plan.md, and tasks.md are created by participants as part of the guided flow in T021; they are not pre-built in the repository. The exercise guide (T021) must include step-by-step instructions for participants to perform these activities.

- [x] T018 [WORKSHOP] [US1] Participants run `/speckit.clarify` during the greenfield flow: clarify identifies underspecified areas in the TodoLite spec, participants answer the targeted questions and encode answers back into the spec, and review the before/after diff as learning evidence — expected artifact: updated spec.md with Clarifications section
- [x] T019 [WORKSHOP] [US1] Participants run `/speckit.checklist` during the greenfield flow: generate a custom verification checklist for the TodoLite feature, review checklist items against acceptance criteria — expected artifact: checklists/ file in the feature spec directory
- [x] T020 [WORKSHOP] [US1] Participants run `/speckit.analyze` during the greenfield flow: run cross-artifact consistency check across spec, plan, and tasks, review findings and resolve any misalignments — expected artifact: analysis report output confirming alignment or listing remediation actions

### Exercise Guide

- [x] T021 [US1] Write greenfield exercise guide in Spanish: MUST include the scenario premise that seeds the live SDD flow (e.g. "Build a minimal TODO CLI called TodoLite that supports add, list, done, and rm commands with JSON file persistence"), step-by-step instructions for participants to generate all SDD artifacts live (init, constitution, specify, clarify, plan, checklist, tasks, analyze, implement), explicit mention that participants create their own feature directory and SDD artifacts from the premise, optional steps demonstration (clarify, checklist, analyze) with expected artifacts, dual-language paths (C#/Python), verification commands with expected output, and reference code fallback instructions pointing to exercises/greenfield/{csharp,python}/ — in materials/02-ejercicio-greenfield.md

**Checkpoint**: Both C# and Python TodoLite CLIs build, tests pass, all 4 commands (add/list/done/rm) produce output matching contracts/greenfield-cli.md. At least one optional Spec Kit step (clarify, checklist, or analyze) has been demonstrated with a visible artifact.

---

## Phase 4: User Story 2 — Customize Spec Kit with presets and extensions (Priority: P2)

**Goal**: Deliver a preset that customizes spec and plan templates and an extension that adds separate dotnet.quickcheck and python.quickcheck commands, with a Spanish module guide. Exercise both customizations through a real SDD cycle and command execution per Constitution Principle I. Both language paths (C# and Python) are covered per Constitution Principle II and FR-005.

**Independent Test**: Install preset via `specify preset add --dev exercises/presets/dotnet-workshop-lite-preset --priority 5`, verify with `specify preset list` and `specify preset resolve spec-template`, then run `/speckit.specify` + `/speckit.plan` and confirm the generated artifacts include the preset sections. Install extension via `specify extension add --dev exercises/extensions/my-ext`, verify with `specify extension list` and `specify extension info my-ext`, then execute `/dotnet.quickcheck` against the C# sample project in exercises/extensions/my-ext/sample/csharp/ and `/python.quickcheck` against the Python sample project in exercises/extensions/my-ext/sample/python/ and confirm both produce a pass/fail summary.

**Applicability**: Hands-on exercise (customization with SDD-flow validation)

### Preset

- [x] T022 [P] [US2] Create preset.yml manifest (id: dotnet-workshop-lite, name, version, description, author, templates list, commands list) and README.md and LICENSE in exercises/presets/dotnet-workshop-lite-preset/
- [x] T023 [P] [US2] Create customized spec-template.md adding a "Verifiable Acceptance Criteria" section with pass/fail checklist format in exercises/presets/dotnet-workshop-lite-preset/templates/spec-template.md
- [x] T024 [P] [US2] Create customized plan-template.md adding a "Key Decisions" table (Decision, Options Considered, Chosen, Rationale) in exercises/presets/dotnet-workshop-lite-preset/templates/plan-template.md
- [x] T025 [P] [US2] Create preset command override speckit.plan.md that includes the decisions table prompt in exercises/presets/dotnet-workshop-lite-preset/commands/speckit.plan.md

### Extension

- [x] T026 [P] [US2] Create extension.yml manifest (id: my-ext, name, version, description, author, templates list, commands list) and README.md and LICENSE in exercises/extensions/my-ext/
- [x] T026b [P] [US2] Create extension-owned spec-template.md that appends a "Risk Assessment" section (risk, likelihood, mitigation) after the core spec sections — in exercises/extensions/my-ext/templates/spec-template.md — this template provides the extensions layer for the FR-016 priority stack demo
- [x] T027 [P] [US2] [C#] Create dotnet.quickcheck.md command that runs dotnet build + dotnet test + reports summary with pass/fail status in exercises/extensions/my-ext/commands/dotnet.quickcheck.md
- [x] T027b [P] [US2] [PY] Create python.quickcheck.md command that runs pip install -e . + pytest + reports summary with pass/fail status in exercises/extensions/my-ext/commands/python.quickcheck.md

### SDD-Flow Execution Validation (Constitution Principle I)

- [x] T029b [US2] Exercise the preset through a real SDD cycle: with the preset installed, run `/speckit.specify` describing "add a health-check endpoint" and then `/speckit.plan`, verify the generated spec contains the "Verifiable Acceptance Criteria" section from the preset's spec-template.md and the generated plan contains the "Key Decisions" table from the preset's plan-template.md — capture before/after evidence showing the customized sections are present
- [x] T029c [US2] [C#] Exercise the .NET extension command against a real project: run `/dotnet.quickcheck` targeting the C# sample project in exercises/extensions/my-ext/sample/csharp/, verify the command executes `dotnet build` + `dotnet test` and produces a pass/fail summary report — expected output: build succeeds, 1 test passed / 0 failed, summary shows PASS — capture the command output as learning evidence
- [x] T029f [P] [US2] [PY] Exercise the Python extension command against a real project: run `/python.quickcheck` targeting the Python sample project in exercises/extensions/my-ext/sample/python/, verify the command executes `pip install -e .` + `pytest` and produces a pass/fail summary report — expected output: install succeeds, 1 test passed / 0 failed, summary shows PASS — capture the command output as learning evidence

### Local Override and Full Priority Stack Demo (FR-016)

- [x] T029d [US2] Create a local override spec-template.md in `.specify/overrides/templates/spec-template.md` that prepends a "Project Banner" header with project name and date — this template provides the overrides (top-priority) layer for the FR-016 priority stack demo
- [x] T029e [US2] Demonstrate the top two layers of the 4-layer template resolution priority stack per FR-016: with preset (T022–T025), extension template (T026b), and local override (T029d) all installed, run `specify preset resolve spec-template` and confirm the resolved output includes the local override's "Project Banner" section (overrides layer wins); then remove only the local override and re-resolve to confirm the preset's "Verifiable Acceptance Criteria" section surfaces (preset layer wins) — capture both resolution outputs as learning evidence. Do NOT remove the preset or extension here; their removal is handled by T028 and T029.

### Rollback and Uninstall Verification (completes the 4-layer walkthrough)

- [x] T028 [US2] Verify preset rollback and extension layer: run `specify preset remove dotnet-workshop-lite`, confirm `specify preset list` no longer shows the preset, run `specify preset resolve spec-template` and confirm the extension's "Risk Assessment" section surfaces (extension layer wins) — document purpose, impact, and rollback procedure in exercises/presets/dotnet-workshop-lite-preset/README.md per Constitution Principle V
- [x] T029 [US2] Verify extension rollback and core layer: run `specify extension remove my-ext`, confirm `specify extension list` no longer shows the extension, run `specify preset resolve spec-template` and confirm the core template is returned (core layer wins), confirm the slash commands are no longer available — document purpose, impact, and rollback procedure in exercises/extensions/my-ext/README.md per Constitution Principle V

### Module Guide

- [x] T030 [US2] Write presets and extensions module guide in Spanish: create preset structure, install/list/resolve, create extension structure with templates and commands (dotnet.quickcheck and python.quickcheck), install/list/info, execute both commands against matching sample projects, SDD-flow exercise with preset (specify + plan with customized templates), extension command execution against real projects (C# and Python), full 4-layer template resolution priority stack walkthrough (overrides → presets → extensions → core) with step-by-step resolve outputs per T029e, rollback procedures (`specify preset remove`, `specify extension remove`, removing local overrides) — in materials/03-extensiones-y-presets.md

**Checkpoint**: Preset installs and resolves customized template; a real specify/plan cycle produces artifacts with the preset's custom sections. Extension installs, provides a template, and both commands produce a pass/fail summary against their matching sample projects in exercises/extensions/my-ext/sample/{csharp,python}/ — C# quickcheck: build succeeds, 1 passed / 0 failed, PASS; Python quickcheck: install succeeds, 1 passed / 0 failed, PASS. Local override takes top priority in resolution. Full 4-layer stack (overrides → presets → extensions → core) is verified with `specify preset resolve` at each layer. Rollback removes all three cleanly. All verification steps and expected outputs are documented in spec.md § Validation Commands.

---

## Phase 5: User Story 3 — Apply SDD to an existing brownfield project (Priority: P3)

**Goal**: Deliver working Notes API reference projects in C# and Python with baseline CRUD endpoints and a search feature endpoint, plus tests (C# and Python) and a Spanish exercise guide. The baseline represents the "pre-existing" code; the search endpoint is the new feature participants implement via SDD.

**Independent Test**: Run `dotnet build && dotnet test && dotnet run` in exercises/brownfield/csharp/, then curl all endpoints. Run `pytest` and `python -m notes_api` in exercises/brownfield/python/, then curl all endpoints. Verify responses match contracts/brownfield-api.md.

**Applicability**: Brownfield

### C# Implementation

- [x] T031 [P] [US3] [C#] Implement Notes API baseline: Note record, CreateNoteRequest record, in-memory ConcurrentDictionary store, tag normalization (trim, lowercase, deduplicate, sort), endpoints GET / (health), GET /notes (list desc), GET /notes/{id} (by ID, 404), POST /notes (create, 201, 400 validation) per contracts/brownfield-api.md in exercises/brownfield/csharp/src/Notes.Api/Program.cs
- [x] T032 [US3] [C#] Implement search endpoint GET /notes/search?q=&tag= with case-insensitive text search on title/body, normalized tag filter, AND logic, SearchResult DTO per contracts/brownfield-api.md in exercises/brownfield/csharp/src/Notes.Api/Program.cs
- [x] T032b [US3] [C#] Implement Notes API tests: health check returns 200, create note returns 201 with normalized tags, create without title returns 400, list returns notes descending, get by ID returns 200/404, search by text/tag/combined returns correct results — in exercises/brownfield/csharp/test/Notes.Api.Tests/NotesApiTests.cs

### Python Implementation

- [x] T033 [P] [US3] [PY] Implement Notes API baseline: Note dataclass, in-memory dict store, tag normalization, Flask routes GET / (health), GET /notes (list desc), GET /notes/<id> (by ID, 404), POST /notes (create, 201, 400 validation), camelCase JSON output, \_\_main\_\_.py entry point per contracts/brownfield-api.md in exercises/brownfield/python/notes_api/app.py and exercises/brownfield/python/notes_api/\_\_main\_\_.py
- [x] T034 [US3] [PY] Implement search endpoint GET /notes/search?q=&tag= with case-insensitive text search, normalized tag filter, AND logic, SearchResult dict per contracts/brownfield-api.md in exercises/brownfield/python/notes_api/app.py
- [x] T035 [US3] [PY] Implement Notes API tests: health check returns 200, create note returns 201 with normalized tags, create without title returns 400, list returns notes descending, get by ID returns 200/404, search by text/tag/combined returns correct results — in exercises/brownfield/python/tests/test_notes_api.py

### Exercise Guide

- [x] T036 [US3] Write brownfield exercise guide in Spanish: MUST include the scenario premise that seeds the live SDD flow (e.g. "You have a working Notes API; add a full-text search endpoint GET /notes/search that filters by query string and tag"), step-by-step instructions for participants to verify the baseline works first, initialize Spec Kit, execute `/speckit.constitution`, surface optional steps (`/speckit.clarify`, `/speckit.checklist`, `/speckit.analyze`) when ambiguities, validation gaps, or cross-artifact risks appear, and generate all SDD artifacts live from the premise (specify search feature, plan, tasks, implement search endpoint), explicit mention that participants create their own feature directory and SDD artifacts from the premise, verify all endpoints work, comparison prompts (greenfield vs brownfield differences), and reference code fallback instructions with copy-and-run steps and links to the C# reference implementation in exercises/brownfield/csharp/ and the Python reference implementation in exercises/brownfield/python/ per FR-014 — in materials/04-ejercicio-brownfield.md

**Checkpoint**: Both C# and Python Notes APIs build/run, baseline endpoints work, search endpoint returns correct filtered results per contracts/brownfield-api.md, C# and Python tests pass.

---

## Phase 6: User Story 4 — Set up and verify the workshop environment (Priority: P4)

**Goal**: Deliver a Spanish preparation guide that participants use to verify all prerequisites before exercises begin.

**Independent Test**: Follow the guide on a clean machine; all verification commands (`specify version`, `specify check`, `git --version`, `python --version`, `dotnet --version`, `uv --version`) return expected output within 5 minutes.

**Applicability**: Content-only (prerequisite for all exercises)

- [x] T037 [US4] Write environment preparation guide in Spanish: shared prerequisites table (Git, Python 3.11+, uv, specify CLI, VS Code + Copilot), per-language runtime table (C# path: .NET 10 SDK; Python path: Python 3.11+ already covered), note that participants validate only their chosen language runtime, installation instructions per OS (Windows, macOS, Linux), verification commands, `specify check` usage, 5-minute target, link to troubleshooting — participant-visible artifact: instruct participants to capture `specify check` output (screenshot or terminal copy) and commit it to their repository as evidence of environment readiness — in materials/00-preparacion.md

**Checkpoint**: A participant following the guide can verify readiness in under 5 minutes.

---

## Phase 7: User Story 5 — Instructor delivers the workshop with structured guidance (Priority: P5)

**Goal**: Deliver Spanish workshop materials for instructor-led delivery: main README, SDD introduction module, troubleshooting guide, and closing retrospective guide.

**Independent Test**: An instructor can conduct a dry run using only the provided materials; all modules have timing, objectives, and deliverables; the troubleshooting guide covers all edge cases from spec.md.

**Applicability**: Content-only (instructor materials)

- [x] T038 [P] [US5] Write main workshop README in Spanish: workshop title, objectives, timing table (preparation 5 min, introduction 15 min, greenfield 45 min, presets/extensions 25 min, brownfield 25 min, closing 5 min), module links, prerequisites summary — in materials/README.md
- [x] T039 [P] [US5] Write SDD introduction module in Spanish: what is SDD, spec vs plan distinction (what/why vs how) with concrete example, Spec Kit core flow diagram (constitution → specify → plan → tasks → implement), optional steps (clarify, analyze, checklist), hands-on demo of `specify init` — in materials/01-introduccion.md
- [x] T040 [P] [US5] Write troubleshooting guide in Spanish: symptom/cause/resolution table covering — specify not in PATH (Windows/macOS/Linux), agent commands not appearing (restart, verify `.specify/commands/` and `.specify/extensions/*/commands/` directories exist and contain `.md` files, re-run init with `--force`), uv or Python not installed, dotnet SDK not found, preset/extension install failures including version incompatibility (verify CLI version with `specify version`, check preset/extension manifest `specifyMinVersion` field, upgrade CLI or pin compatible version), brownfield scan too slow, divergent exercise code, wrong branch or detached HEAD (git checkout/switch recovery), feature-branch naming mismatch (expected convention vs actual, rename with `git branch -m`), uncommitted changes blocking branch switch (`git stash` / `git stash pop`) — per spec edge cases and FR-011 — each resolution MUST be specific and actionable enough for a participant to apply in under 3 minutes (SC-010) — in materials/troubleshooting.md
- [x] T041 [US5] Write closing retrospective guide in Spanish: reflection prompts (which SDD phase added most clarity, how ambiguities were resolved, greenfield vs brownfield comparison, team customization needs), next steps, resources — in materials/05-cierre.md
- [x] T041b [US5] Create a fill-in retrospective artifact template in Spanish per FR-015: structured Markdown file with sections for key decisions made during exercises (decision, options considered, chosen option, rationale), trade-offs observed (greenfield vs brownfield), lessons learned, and concrete next steps for adopting SDD in the participant's team — participants fill in the template during the closing module and commit it to their repository as evidence of learning — participant-visible artifact: the completed retrospectiva.md committed to the repository serves as the content-only module's tangible deliverable per FR-009 SHOULD — in materials/retrospectiva.md

**Checkpoint**: All 9 materials files exist (README.md, 00-preparacion.md, 01-introduccion.md, 02-ejercicio-greenfield.md, 03-extensiones-y-presets.md, 04-ejercicio-brownfield.md, 05-cierre.md, retrospectiva.md, troubleshooting.md), timing totals ~120 min, troubleshooting covers all 6 spec edge cases plus Git branch/feature errors (FR-011).

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Validate all deliverables end-to-end, verify parity, and ensure materials consistency.

- [x] T042 [P] Run quickstart.md validation commands for all 4 reference projects: greenfield C# (dotnet build, dotnet test, CLI commands), greenfield Python (pip install, pytest, CLI commands), brownfield C# (dotnet build, dotnet test, dotnet run, curl endpoints), brownfield Python (pip install, pytest, python -m notes_api, curl endpoints)
- [x] T043 Verify behavioral parity between C# and Python for both exercises: identical JSON field names (camelCase), matching exit codes (greenfield), matching HTTP status codes (brownfield), same tag normalization, same search logic
- [x] T044 Final review of all materials/ files for Spanish language consistency, cross-references between modules, and links to reference code in exercises/
- [x] T045 Language-policy compliance audit per Constitution Principle VI, FR-012, and FR-013: verify all source code files, SDD artifacts (constitution, spec, plan, tasks), code comments, commit messages, technical documentation, and project-level README.md files under exercises/ and specs/ are written exclusively in English — flag any Spanish fragments in English-scoped artifacts and any English prose in participant-facing Spanish materials under materials/
- [x] T046 [US5] Timed troubleshooting rehearsal per SC-010: select at least 3 representative issues from the troubleshooting guide (one CLI/environment, one agent/commands, one preset/extension), simulate each issue, follow the documented resolution steps, and verify each can be resolved in under 3 minutes — record actual times and flag any resolution that exceeds the target for revision

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: N/A — no blocking prerequisites for this feature
- **User Stories (Phase 3–7)**: All depend on Setup (Phase 1) completion
  - US1, US3, US4, US5 can proceed in parallel (independent deliverables)
  - US2 materials reference US1 contextually but code artifacts are independent
- **Polish (Phase 8)**: Depends on all user stories being complete

### User Story Dependencies

- **US1 (P1)**: Can start after Setup — no dependencies on other stories
- **US2 (P2)**: Can start after Setup — fully independent; uses its own sample project in exercises/extensions/my-ext/sample/ for quickcheck validation
- **US3 (P3)**: Can start after Setup — no dependencies on other stories
- **US4 (P4)**: Can start after Setup — references exercise verification commands but no code dependencies
- **US5 (P5)**: Can start after Setup — troubleshooting guide references all exercises; best written after US1/US3 code is stable

### Within Each User Story

- C# track and Python track can run in parallel (different directory trees)
- Models/entities before services/facades
- Services before CLI entry points / API endpoints
- Implementation before tests (tests validate the implementation)
- Code before exercise guide (guide references verification commands and expected output)

### Parallel Opportunities

- **Phase 1**: T002–T007c are all parallel (independent project scaffolds)
- **Phase 3 (US1)**: C# track (T008–T012) parallel with Python track (T013–T017); optional steps interleave with the participant’s live SDD flow: T018 (clarify) after specify and before plan, T019 (checklist) after plan and before tasks, T020 (analyze) after tasks and before implement
- **Phase 4 (US2)**: T007b, T007c, T022–T027b (including T026b) are all parallel (independent files); T029b–T029c and T029f run after install (require installed preset/extension and US2-owned sample projects from T007b/T007c); T029d creates the local override (parallel with T029b–T029c/T029f); T029e demonstrates override and preset layers (sequential, after T029b–T029d); T028 removes preset and verifies extension layer (sequential, after T029e); T029 removes extension and verifies core layer (sequential, after T028)
- **Phase 5 (US3)**: C# track (T031–T032, T032b) parallel with Python track (T033–T035); optional steps are surfaced in T036 as decision points during the brownfield live SDD flow when clarification, verification, or cross-artifact review is useful
- **Phase 7 (US5)**: T038–T040 are all parallel (independent documents)

## Implementation Strategy

### MVP Scope

**User Story 1 (Greenfield)** is the MVP. It delivers the foundational learning experience — the complete SDD pipeline end-to-end — and is independently valuable even if later stories are deferred.

### Incremental Delivery Order

1. **Phase 1** → Project scaffolding (all stories unblocked)
2. **US1** → Greenfield C# + Python + exercise guide (core workshop value)
3. **US3** → Brownfield C# + Python + exercise guide (second exercise)
4. **US2** → Presets and extensions (customization module)
5. **US4** → Environment preparation guide
6. **US5** → Instructor materials, troubleshooting, closing
7. **Phase 8** → End-to-end validation and parity checks
