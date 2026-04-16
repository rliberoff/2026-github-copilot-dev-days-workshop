"""JSON file repository for TodoLite."""

from __future__ import annotations

import json
from pathlib import Path

from todolite.models import TodoItem


class JsonFileTodoRepository:
    """Persists TodoItem instances to a JSON file."""

    def __init__(self, file_path: str = "todolite.json") -> None:
        self.file_path = Path(file_path)

    def load(self) -> list[TodoItem]:
        """Load all items from the JSON file."""
        if not self.file_path.exists():
            return []

        data = json.loads(self.file_path.read_text(encoding="utf-8"))
        return [TodoItem.from_dict(item) for item in data]

    def save(self, items: list[TodoItem]) -> None:
        """Save all items to the JSON file."""
        data = [item.to_dict() for item in items]
        self.file_path.write_text(
            json.dumps(data, indent=2, ensure_ascii=False),
            encoding="utf-8",
        )
