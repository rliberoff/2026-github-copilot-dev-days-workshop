# Data Model: Spec Kit Introductory Workshop

**Feature**: `001-speckit-workshop`
**Date**: 2026-04-13
**Phase**: 1 — Design & Contracts

## Greenfield Exercise: TodoLite

### Entity: TodoItem

Represents a single task in the to-do list.

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | integer | Positive, auto-incremented, unique within file | Assigned sequentially starting from 1 |
| Text | string | Non-empty after trimming | The task description |
| IsDone | boolean | Default: false | Completion status flag |
| CreatedAtUtc | datetime with offset | Set on creation, immutable | UTC timestamp of creation |
| DoneAtUtc | datetime with offset, nullable | Set when marked done, null otherwise | UTC timestamp of completion |

**Validation rules**:

- `Text` must not be empty or whitespace-only after trimming.
- `Id` is derived from the current maximum ID in the store + 1. If the store is empty, the first ID is 1.

**State transitions**:

- `Open` → `Done`: sets `IsDone = true` and `DoneAtUtc` to current UTC time. Idempotent (marking an already-done item returns success).
- `Open` or `Done` → `Removed`: item is deleted from the store. No soft-delete.

**Persistence**: JSON file (`todolite.json` by default). The file contains a JSON array of TodoItem objects. If the file does not exist, the store is treated as empty.

### Operations

| Operation | Input | Output | Side Effect |
|-----------|-------|--------|-------------|
| Add | text (string) | Created TodoItem | Appends to JSON file |
| List | optional: --open flag | Ordered list of TodoItems | None (read-only) |
| MarkDone | id (integer) | Success/not-found boolean | Updates JSON file |
| Remove | id (integer) | Success/not-found boolean | Updates JSON file |

## Brownfield Exercise: Notes API

### Entity: Note

Represents a single note in the in-memory store.

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | GUID/UUID | Auto-generated, unique | Primary identifier |
| Title | string | Non-empty after trimming | Required note title |
| Body | string | May be empty | Optional note body |
| Tags | array of strings | Normalized: trimmed, lowercased, deduplicated, sorted | Categorization labels |
| CreatedAtUtc | datetime with offset | Set on creation, immutable | UTC timestamp of creation |

**Validation rules**:

- `Title` is required and must not be empty or whitespace-only.
- `Body` defaults to empty string if not provided.
- `Tags` normalization: each tag is trimmed, empty tags are removed, remaining are lowercased, deduplicated, and sorted alphabetically.

**Persistence**: In-memory only (ConcurrentDictionary in C#, plain dict in Python). Data does not survive process restart. This is intentional for a workshop exercise.

### Entity: CreateNoteRequest

Input DTO for creating a note.

| Field | Type | Required |
|-------|------|----------|
| Title | string | Yes |
| Body | string | No |
| Tags | array of strings | No |

### Entity: SearchResult

Output DTO for the search endpoint (brownfield feature).

| Field | Type | Notes |
|-------|------|-------|
| query | string, nullable | The text query used, null if none |
| tag | string, nullable | The tag filter used, null if none |
| count | integer | Number of matching notes |
| items | array of Note | Matching notes, ordered by CreatedAtUtc descending |

### Operations

| Operation | Endpoint | Input | Output |
|-----------|----------|-------|--------|
| Health check | GET / | None | "Notes API OK" |
| List notes | GET /notes | None | Array of Note, ordered by CreatedAtUtc desc |
| Get note | GET /notes/{id} | id (GUID) | Note or 404 |
| Create note | POST /notes | CreateNoteRequest (JSON body) | 201 Created with Note |
| Search notes | GET /notes/search | q (string, optional), tag (string, optional) | SearchResult |

**Search behavior**:

- If both `q` and `tag` are empty/missing: returns all notes.
- If `q` has a value: filters notes where Title or Body contains the query (case-insensitive).
- If `tag` has a value: filters notes that have the tag (normalized, case-insensitive match).
- Both filters combine with AND logic.

## Workshop Instructional Entities

### Entity: WorkshopModule

| Field | Type | Notes |
|-------|------|-------|
| Name | string | Module identifier (e.g., "greenfield", "brownfield") |
| Duration | integer (minutes) | Allocated time |
| Objective | string | Learning goal |
| Deliverable | string | Tangible artifact produced |

### Modules

| Name | Duration | Deliverable |
|------|----------|-------------|
| Environment Verification | 5 min | `specify check` validated |
| Introduction | 15 min | Conceptual map of SDD flow |
| Greenfield Exercise | 45 min | Working app + 4 SDD artifacts |
| Presets and Extensions | 25 min | Installed preset + extension |
| Brownfield Exercise | 25 min | New feature on existing API |
| Closing | 5 min | Retrospective notes |

### Entity: TroubleshootingEntry

| Field | Type | Notes |
|-------|------|-------|
| Symptom | string | What the participant observes |
| ProbableCause | string | Most likely root cause |
| Resolution | string | Step-by-step fix |
