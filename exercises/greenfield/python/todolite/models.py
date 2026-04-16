"""Data model for TodoLite."""

from __future__ import annotations

from dataclasses import dataclass
from datetime import datetime, timezone


@dataclass(frozen=True)
class TodoItem:
    """Represents a single task in the to-do list."""

    id: int
    text: str
    is_done: bool
    created_at_utc: datetime
    done_at_utc: datetime | None

    @staticmethod
    def create(item_id: int, text: str) -> TodoItem:
        """Create a new open TodoItem."""
        return TodoItem(
            id=item_id,
            text=text,
            is_done=False,
            created_at_utc=datetime.now(timezone.utc),
            done_at_utc=None,
        )

    def complete(self) -> TodoItem:
        """Return a copy marked as done."""
        return TodoItem(
            id=self.id,
            text=self.text,
            is_done=True,
            created_at_utc=self.created_at_utc,
            done_at_utc=datetime.now(timezone.utc),
        )

    def to_dict(self) -> dict:
        """Serialize to a camelCase dictionary for JSON persistence."""
        return {
            "id": self.id,
            "text": self.text,
            "isDone": self.is_done,
            "createdAtUtc": self.created_at_utc.isoformat(),
            "doneAtUtc": self.done_at_utc.isoformat() if self.done_at_utc else None,
        }

    @staticmethod
    def from_dict(data: dict) -> TodoItem:
        """Deserialize from a camelCase dictionary."""
        return TodoItem(
            id=data["id"],
            text=data["text"],
            is_done=data["isDone"],
            created_at_utc=datetime.fromisoformat(data["createdAtUtc"]),
            done_at_utc=datetime.fromisoformat(data["doneAtUtc"]) if data.get("doneAtUtc") else None,
        )
