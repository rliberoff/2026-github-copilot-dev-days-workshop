# Contract: Brownfield API (Notes API)

**Feature**: `001-speckit-workshop`
**Date**: 2026-04-13

## Interface Type

HTTP REST API — served via ASP.NET Minimal API (C#) or Flask (Python).

## Base URL

`http://localhost:5000` (default for both implementations).

## Endpoints

### GET /

Health check.

- **Response**: `200 OK`, body: `Notes API OK` (plain text).

### GET /notes

List all notes.

- **Response**: `200 OK`, JSON array of Note objects ordered by `createdAtUtc` descending.

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Buy milk",
    "body": "Semi-skimmed",
    "tags": ["groceries", "home"],
    "createdAtUtc": "2026-04-13T10:00:00+00:00"
  }
]
```

### GET /notes/{id}

Get a single note by ID.

- **Path parameter**: `id` — UUID/GUID.
- **Response (found)**: `200 OK`, JSON Note object.
- **Response (not found)**: `404 Not Found`, JSON `{ "message": "Note not found." }`.

### POST /notes

Create a new note.

- **Request body** (JSON):

```json
{
  "title": "Buy milk",
  "body": "Semi-skimmed",
  "tags": ["Groceries", "Home"]
}
```

- **Response (success)**: `201 Created`, Location header: `/notes/{id}`, JSON Note object with normalized tags.
- **Response (validation error)**: `400 Bad Request`, JSON `{ "message": "Title is required." }`.

### GET /notes/search

Search notes by text and/or tag. **This is the brownfield feature to implement.**

- **Query parameters**:
  - `q` (string, optional): Text search query.
  - `tag` (string, optional): Tag filter.
- **Response**: `200 OK`, JSON SearchResult object.

```json
{
  "query": "milk",
  "tag": "groceries",
  "count": 1,
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "title": "Buy milk",
      "body": "Semi-skimmed",
      "tags": ["groceries", "home"],
      "createdAtUtc": "2026-04-13T10:00:00+00:00"
    }
  ]
}
```

**Search behavior**:

- Both `q` and `tag` empty/missing → returns all notes.
- `q` present → filters where `title` or `body` contains `q` (case-insensitive).
- `tag` present → filters notes containing the tag (normalized, case-insensitive).
- Both present → AND logic.
- Results ordered by `createdAtUtc` descending.

## JSON Field Naming

All JSON responses use **camelCase** field names for both C# and Python implementations.

## Verification Commands

### C\#

```bash
cd exercises/brownfield/csharp
dotnet build
dotnet run --project src/Notes.Api &

# Create a note
curl -s -X POST "http://localhost:5000/notes" \
  -H "Content-Type: application/json" \
  -d '{"title":"Buy milk","body":"Semi-skimmed","tags":["groceries","home"]}'

# List notes
curl -s "http://localhost:5000/notes"

# Search by text
curl -s "http://localhost:5000/notes/search?q=milk"

# Search by tag
curl -s "http://localhost:5000/notes/search?tag=groceries"

# Combined search
curl -s "http://localhost:5000/notes/search?q=milk&tag=groceries"
```

### Python

```bash
cd exercises/brownfield/python
pip install -e ".[dev]"
pytest
python -m notes_api &

# Same curl commands as above
```

## Behavioral Parity

Both C# and Python implementations MUST:

- Respond with identical JSON structure and field names (camelCase).
- Use the same HTTP status codes for the same scenarios.
- Apply the same tag normalization rules (trim, lowercase, deduplicate, sort).
- Apply the same search logic (case-insensitive contains for text, normalized match for tag, AND combination).
