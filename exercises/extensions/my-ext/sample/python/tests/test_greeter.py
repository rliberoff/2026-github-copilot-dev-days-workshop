"""Tests for the sample greeter."""

from sample_greeter.greeter import Greeter


def test_greet_returns_expected_message():
    greeter = Greeter()

    result = greeter.greet("World")

    assert result == "Hello, World!"
