# AGENTS.md

Guidance for AI coding agents (GitHub Copilot, Claude Code, and others) working in this repository.

## Project

- `FirstApplication`: ASP.NET Core Web API, .NET 10, nullable enabled, implicit usings.
- `FirstApplication.Tests`: xUnit v3 + Moq.
- Solution file: `FirstApplication.slnx`.

Layout: `Controllers/`, `Services/`, `Models/`, `Configuration/`, `Middleware/`, `Filters/`, `Attributes/`, `Examples/`.

## Commands

```
dotnet build FirstApplication.slnx -nologo -v q
dotnet test FirstApplication.slnx -nologo
dotnet format whitespace FirstApplication.slnx --include <files>
```

Run build and tests before declaring work done.

## Conventions

- One top-level type per file; the file name matches the type. Namespace is `FirstApplication.<Folder>`.
- Main project uses block-scoped namespaces; the test project uses file-scoped namespaces.
- 4-space indentation, Allman braces, always use braces on `if`/`for`/`foreach`/`while`.
- Member order: constants, static readonly fields, readonly fields, constructors, public methods, private methods.
- Private fields are `_camelCase`; async methods end with `Async`; catch variables are named `exception`.
- Seal classes not designed for inheritance (services, middleware, options, test classes).
- Guard reference-type constructor arguments with `ArgumentNullException.ThrowIfNull(...)`, then a blank line, then assignments.
- Keep methods small (about 20 lines, one level of nesting); extract well-named private helpers.
- Log with structured templates (`{StatusCode}`), never interpolation. Pass exceptions as the first argument.
- Controllers: `[ApiController]`, `[Route("[controller]")]`, inherit `ControllerBase`, inject interfaces, return `ActionResult<T>`.
- Public async actions accept a `CancellationToken` and pass it down.
- Return `IReadOnlyList<T>` from public APIs rather than `List<T>` or arrays.

## Tests

- `public sealed class <TypeUnderTest>Tests`; methods named `Method_ExpectedBehaviour[_Condition]`.
- `[Fact]` for single cases, `[Theory]` + `[InlineData]` when only inputs differ.
- Arrange / Act / Assert separated by blank lines. Shared fakes live in `TestDoubles/`.
- New behaviour needs tests.

## Do not

- Edit `bin/`, `obj/`, `.vs/`, `TestResults/`, or generated files.
- Change public signatures or behaviour while doing formatting-only work.
- Commit secrets or `.claude/settings.local.json`.

## Agents, skills and tools

Every agent, whether GitHub Copilot or Claude Code, uses the same agents, skills and tool sets.

| Agent | Purpose | Tool set |
|---|---|---|
| `pr-reviewer` | Review diffs; read-only | read-only |
| `code-formatter` | Apply conventions after C# edits | read/edit |
| `expert-dotnet-engineer` | Design, implement and test .NET code | read/edit |

Tool sets map across the two products:

| Capability | Copilot (`.github/agents/*.agent.md`) | Claude Code (`.claude/agents/*.md`) |
|---|---|---|
| read-only | `read`, `search`, `execute` | `Read`, `Grep`, `Glob`, `Bash`, `PowerShell` |
| read/edit | `read`, `search`, `edit`, `execute` | `Read`, `Grep`, `Glob`, `Edit`, `Write`, `Bash`, `PowerShell` |

- Agent definitions: `.github/agents/` for Copilot and `.claude/agents/` for Claude. Keep the instructions identical and change only the frontmatter. If you edit one, edit the other.
- Skills: the canonical copy is `.github/skills/`, mirrored to `.claude/skills/`. Edit `.github/skills/` only, then run `scripts/sync-agent-skills.ps1`.
- Skills come from the [csharp-dotnet-development](https://github.com/github/awesome-copilot/tree/main/plugins/csharp-dotnet-development) plugin of awesome-copilot (MIT): `csharp-async`, `csharp-xunit`, `dotnet-best-practices`, `aspnet-minimal-api-openapi`, `dotnet-upgrade`, `csharp-mstest`, `csharp-nunit`, `csharp-tunit`, `resx-source-generator-migration`. This repo uses xUnit, so ignore the other test-framework skills. Its `expert-dotnet-software-engineer` agent is included as `expert-dotnet-engineer`.
