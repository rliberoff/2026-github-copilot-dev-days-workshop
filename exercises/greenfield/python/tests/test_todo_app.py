"""Tests for the TodoLite application."""

from __future__ import annotations

import os
import tempfile

import pytest

from todolite import TodoApp
from todolite.repository import JsonFileTodoRepository


@pytest.fixture
def app(tmp_path):
    """Create a TodoApp with a temporary JSON file."""
    file_path = str(tmp_path / "test-todolite.json")
    repository = JsonFileTodoRepository(file_path)
    return TodoApp(repository)


def test_add_creates_task(app):
    item = app.add("Buy milk")
    assert item.id == 1
    assert item.text == "Buy milk"
    assert item.is_done is False
    assert item.done_at_utc is None


def test_list_returns_all_tasks(app):
    app.add("Task one")
    app.add("Task two")
    items = app.list()
    assert len(items) == 2


def test_list_open_only_filters_done_tasks(app):
    app.add("Open task")
    done = app.add("Done task")
    app.mark_done(done.id)

    items = app.list(open_only=True)
    assert len(items) == 1
    assert items[0].text == "Open task"


def test_mark_done_marks_task_as_complete(app):
    item = app.add("Test task")
    result = app.mark_done(item.id)
    assert result is True
    items = app.list()
    assert items[0].is_done is True
    assert items[0].done_at_utc is not None


def test_mark_done_is_idempotent(app):
    item = app.add("Test task")
    app.mark_done(item.id)
    result = app.mark_done(item.id)
    assert result is True


def test_remove_removes_task(app):
    item = app.add("Test task")
    result = app.remove(item.id)
    assert result is True
    assert len(app.list()) == 0


def test_mark_done_not_found_returns_false(app):
    result = app.mark_done(999)
    assert result is False


def test_remove_not_found_returns_false(app):
    result = app.remove(999)
    assert result is False


def test_add_empty_text_raises_value_error(app):
    with pytest.raises(ValueError):
        app.add("   ")
