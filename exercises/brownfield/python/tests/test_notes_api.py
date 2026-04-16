"""Tests for the Notes API."""

from __future__ import annotations

import json

import pytest

from notes_api.app import create_app


@pytest.fixture
def client():
    """Create a test client for the Notes API."""
    app = create_app()
    app.config["TESTING"] = True
    with app.test_client() as test_client:
        yield test_client


def test_health_check_returns_200(client):
    response = client.get("/")
    assert response.status_code == 200
    assert b"Notes API OK" in response.data


def test_create_note_returns_201_with_normalized_tags(client):
    data = {"title": "Buy milk", "body": "Semi-skimmed", "tags": ["  Groceries  ", "HOME", "groceries"]}
    response = client.post("/notes", json=data)

    assert response.status_code == 201
    note = response.get_json()
    assert note["tags"] == ["groceries", "home"]
    assert note["title"] == "Buy milk"


def test_create_note_without_title_returns_400(client):
    response = client.post("/notes", json={"body": "No title"})
    assert response.status_code == 400


def test_list_notes_returns_descending_order(client):
    client.post("/notes", json={"title": "First"})
    client.post("/notes", json={"title": "Second"})

    response = client.get("/notes")
    assert response.status_code == 200
    notes = response.get_json()
    assert len(notes) >= 2
    assert notes[0]["title"] == "Second"


def test_get_note_by_id_returns_200(client):
    create_response = client.post("/notes", json={"title": "Fetch me"})
    note = create_response.get_json()

    response = client.get(f"/notes/{note['id']}")
    assert response.status_code == 200
    assert response.get_json()["title"] == "Fetch me"


def test_get_note_by_id_not_found_returns_404(client):
    response = client.get("/notes/00000000-0000-0000-0000-000000000000")
    assert response.status_code == 404
