---
name: speckit.my-ext.python-quickcheck
description: "Run pip install and pytest against the current Python project and report a pass/fail summary"
---

## Python Quick Check

This command verifies a Python project by running install and test steps, then producing a summary report.

### Steps

1. **Locate the project**: Find the nearest `pyproject.toml` file in the current workspace or use the path provided by the user.

2. **Install**: Run the following command from the project directory:

   ```bash
   pip install -e ".[dev]" --quiet
   ```

   - If the install fails, report **FAIL** with the error output and stop.

3. **Test**: Run the following command from the project directory:

   ```bash
   python -m pytest tests/ -v --tb=short
   ```

   - Capture the test results summary (passed, failed, skipped counts).

4. **Report**: Produce a summary in the following format:

   ```
   ## Python Quick Check Report

   **Project**: {project name from pyproject.toml}
   **Install**: PASS | FAIL
   **Tests**: {passed} passed / {failed} failed / {skipped} skipped
   **Result**: PASS | FAIL

   {If FAIL, include relevant error details}
   ```

### Expected Output (Sample Project)

When run against the sample project in `exercises/extensions/my-ext/sample/python/`:

```
## Python Quick Check Report

**Project**: sample-greeter
**Install**: PASS
**Tests**: 1 passed / 0 failed / 0 skipped
**Result**: PASS
```
