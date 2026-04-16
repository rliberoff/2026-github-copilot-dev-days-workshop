---
name: speckit.my-ext.dotnet-quickcheck
description: "Run dotnet build and dotnet test against the current .NET project and report a pass/fail summary"
---

## .NET Quick Check

This command verifies a .NET project by running build and test steps, then producing a summary report.

### Steps

1. **Locate the project**: Find the nearest `.sln` file in the current workspace or use the path provided by the user.

2. **Build**: Run the following command from the solution directory:

   ```bash
   dotnet build --nologo --verbosity quiet
   ```

   - If the build fails, report **FAIL** with the error output and stop.

3. **Test**: Run the following command from the solution directory:

   ```bash
   dotnet test --nologo --verbosity quiet
   ```

   - Capture the test results summary (passed, failed, skipped counts).

4. **Report**: Produce a summary in the following format:

   ```
   ## .NET Quick Check Report

   **Project**: {solution name}
   **Build**: PASS | FAIL
   **Tests**: {passed} passed / {failed} failed / {skipped} skipped
   **Result**: PASS | FAIL

   {If FAIL, include relevant error details}
   ```

### Expected Output (Sample Project)

When run against the sample project in `exercises/extensions/my-ext/sample/csharp/`:

```
## .NET Quick Check Report

**Project**: Sample.sln
**Build**: PASS
**Tests**: 1 passed / 0 failed / 0 skipped
**Result**: PASS
```
