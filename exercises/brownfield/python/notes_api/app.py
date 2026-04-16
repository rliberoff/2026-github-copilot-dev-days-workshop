"""Flask application for the Notes API."""

from __future__ import annotations

import uuid
from dataclasses import dataclass, field
from datetime import datetime, timezone

from flask import Flask, jsonify, request


@dataclass
class Note:
    """Represents a note in the store."""

    id: str
    title: str
    body: str
    tags: list[str]
    created_at_utc: datetime

    def to_dict(self) -> dict:
        """Serialize to camelCase dictionary."""
        return {
            "id": self.id,
            "title": self.title,
            "body": self.body,
            "tags": self.tags,
            "createdAtUtc": self.created_at_utc.isoformat(),
        }


def normalize_tags(tags: list[str] | None) -> list[str]:
    """Trim, lowercase, deduplicate, and sort tags."""
    if not tags:
        return []
    seen: set[str] = set()
    result: list[str] = []
    for tag in tags:
        normalized = tag.strip().lower()
        if normalized and normalized not in seen:
            seen.add(normalized)
            result.append(normalized)
    return sorted(result)


def create_app() -> Flask:
    """Create and configure the Flask application."""
    app = Flask(__name__)

    store: dict[str, Note] = {}

    @app.get("/")
    def health_check():
        return "Notes API OK", 200

    @app.get("/notes")
    def list_notes():
        notes = sorted(store.values(), key=lambda n: n.created_at_utc, reverse=True)
        return jsonify([n.to_dict() for n in notes])

    @app.get("/notes/<note_id>")
    def get_note(note_id: str):
        note = store.get(note_id)
        if note is None:
            return jsonify({"message": "Note not found."}), 404
        return jsonify(note.to_dict())

    @app.post("/notes")
    def create_note():
        data = request.get_json(silent=True) or {}
        title = data.get("title")

        if not title or not title.strip():
            return jsonify({"message": "Title is required."}), 400

        note_id = str(uuid.uuid4())
        tags = normalize_tags(data.get("tags"))
        note = Note(
            id=note_id,
            title=title.strip(),
            body=(data.get("body") or "").strip(),
            tags=tags,
            created_at_utc=datetime.now(timezone.utc),
        )
        store[note_id] = note

        response = jsonify(note.to_dict())
        response.status_code = 201
        response.headers["Location"] = f"/notes/{note_id}"
        return response

    return app
