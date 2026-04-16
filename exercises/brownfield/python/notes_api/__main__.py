"""Entry point for running the Notes API."""

from notes_api.app import create_app


def main() -> None:
    """Run the Flask development server."""
    application = create_app()
    application.run(host="0.0.0.0", port=5000)


if __name__ == "__main__":
    main()
