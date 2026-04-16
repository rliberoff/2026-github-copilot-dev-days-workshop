"""TodoLite — A minimal to-do CLI application."""

from todolite.models import TodoItem
from todolite.repository import JsonFileTodoRepository

__all__ = ["TodoApp", "TodoItem", "JsonFileTodoRepository"]


class TodoApp:
    """Application service for managing to-do items."""

    def __init__(self, repository: JsonFileTodoRepository) -> None:
        self.repository = repository

    def add(self, text: str) -> TodoItem:
        """Create a new task with the given text."""
        trimmed = text.strip()
        if not trimmed:
            raise ValueError("Task text must not be empty.")

        items = self.repository.load()
        next_id = max((i.id for i in items), default=0) + 1
        item = TodoItem.create(next_id, trimmed)
        items.append(item)
        self.repository.save(items)
        return item

    def list(self, *, open_only: bool = False) -> list[TodoItem]:
        """List all tasks, or only open tasks if open_only is True."""
        items = self.repository.load()
        if open_only:
            items = [i for i in items if not i.is_done]
        return sorted(items, key=lambda i: i.id)

    def mark_done(self, task_id: int) -> bool:
        """Mark a task as done. Idempotent. Returns True if found."""
        items = self.repository.load()
        for idx, item in enumerate(items):
            if item.id == task_id:
                if not item.is_done:
                    items[idx] = item.complete()
                    self.repository.save(items)
                return True
        return False

    def remove(self, task_id: int) -> bool:
        """Remove a task permanently. Returns True if found."""
        items = self.repository.load()
        new_items = [i for i in items if i.id != task_id]
        if len(new_items) < len(items):
            self.repository.save(new_items)
            return True
        return False
