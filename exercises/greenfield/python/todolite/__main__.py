"""CLI entry point for TodoLite."""

from __future__ import annotations

import argparse
import sys

from todolite import TodoApp
from todolite.repository import JsonFileTodoRepository


def main(argv: list[str] | None = None) -> int:
    """Parse arguments and dispatch to the appropriate command."""
    parser = argparse.ArgumentParser(prog="todolite", description="A minimal to-do CLI")
    parser.add_argument("--file", default="todolite.json", help="Path to the JSON persistence file")
    subparsers = parser.add_subparsers(dest="command")

    # add
    add_parser = subparsers.add_parser("add", help="Create a new task")
    add_parser.add_argument("text", nargs="+", help="Task text")

    # list
    list_parser = subparsers.add_parser("list", help="List all tasks")
    list_parser.add_argument("--open", action="store_true", dest="open_only", help="Show only open tasks")

    # done
    done_parser = subparsers.add_parser("done", help="Mark a task as done")
    done_parser.add_argument("id", type=int, help="Task ID")

    # rm
    rm_parser = subparsers.add_parser("rm", help="Remove a task")
    rm_parser.add_argument("id", type=int, help="Task ID")

    # help
    subparsers.add_parser("help", help="Show help")

    args = parser.parse_args(argv)

    if args.command is None or args.command == "help":
        parser.print_help()
        return 2

    repository = JsonFileTodoRepository(args.file)
    app = TodoApp(repository)

    if args.command == "add":
        text = " ".join(args.text)
        item = app.add(text)
        print(f"Created task #{item.id}: {item.text}")
        return 0

    if args.command == "list":
        items = app.list(open_only=args.open_only)
        for item in items:
            check = "x" if item.is_done else " "
            print(f"[{check}] {item.id:3}  {item.text}")
        return 0

    if args.command == "done":
        if app.mark_done(args.id):
            print(f"OK: task #{args.id} marked as done.")
            return 0
        print(f"Task #{args.id} does not exist.")
        return 1

    if args.command == "rm":
        if app.remove(args.id):
            print(f"OK: task #{args.id} removed.")
            return 0
        print(f"Task #{args.id} does not exist.")
        return 1

    parser.print_help()
    return 2


if __name__ == "__main__":
    sys.exit(main())
