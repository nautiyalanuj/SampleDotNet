# Copilot instructions

This is an ASP.NET Core Web API (.NET 10) with an xUnit v3 + Moq test project.

The full conventions, commands and layout are in [AGENTS.md](../AGENTS.md). Follow them for every change.

Key points:

- Build and test with `dotnet build FirstApplication.slnx` and `dotnet test FirstApplication.slnx` before finishing.
- Block-scoped namespaces in the main project, file-scoped in the test project.
- Guard constructor arguments with `ArgumentNullException.ThrowIfNull`.
- Structured logging only; no string interpolation in log calls.
- Async actions take and forward a `CancellationToken`.
- Add or update tests for any behaviour change.
- Skills live in `.github/skills/` (mirrored to `.claude/skills/` for Claude Code); prefer `csharp-async`, `csharp-xunit` and `dotnet-best-practices`.
- Use the `expert-dotnet-engineer` agent for design and implementation.
- Use the `pr-reviewer` agent for reviews and the `code-formatter` agent after editing C# files.
