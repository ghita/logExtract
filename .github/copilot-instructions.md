# Copilot Instructions for logExtract

## Project Overview
- **logExtract** is a C#/.NET solution for extracting and processing Windows Event Logs and exporting them to CSV or other formats.
- The solution is split into three main projects:
  - `logExtract/`: Core library for event log extraction and CSV export logic.
  - `logExtract-tool/`: Command-line tool for running extraction jobs using the core library.
  - `logExtract.Test/`: xUnit-based test project for validating core logic.

## Architecture & Key Files
- **Core Logic**: `EventLogExtractor.cs`, `CSVExtract.cs`, `ExtractedEvent.cs`, `IExtractedEvents.cs` (in `logExtract/`)
- **CLI Entrypoint**: `Program.cs` (in `logExtract-tool/`)
- **Tests**: `UnitTest1.cs` (in `logExtract.Test/`)
- **Solution/Project Files**: `logExtract.sln`, `*.csproj`

## Developer Workflows
- **Build**: Run `dotnet build logExtract.sln` from the repo root.
- **Test**: Run `dotnet test logExtract.sln` or use the test explorer in VS Code/Visual Studio.
- **Run CLI Tool**: `dotnet run --project logExtract-tool` (add arguments as needed for extraction jobs).
- **Debug**: Attach to the CLI tool or run with `dotnet run` in debug mode.

## Patterns & Conventions
- **Separation of Concerns**: Core extraction logic is in the library; CLI is a thin wrapper.
- **Interfaces**: Use `IExtractedEvents` for abstraction and testability.
- **Data Model**: `ExtractedEvent` represents a single event log entry.
- **Testing**: Use xUnit for all tests. Place new tests in `logExtract.Test/`.
- **.NET Version**: Targeting .NET 9.0 (see `csproj` files).

## Integration & Dependencies
- **System.Diagnostics.EventLog**: Used for reading Windows Event Logs.
- **CliWrap**: Used in the CLI tool for process management.
- **No external DB or web dependencies** (local file and event log access only).

## Examples
- To add a new extraction format, implement a new class in `logExtract/` and update the CLI tool to use it.
- To add a new test, create a new file in `logExtract.Test/` and use xUnit `[Fact]` or `[Theory]` attributes.

## Special Notes
- All cross-project references are managed via the solution and project files.
- Keep CLI logic minimal; business logic belongs in the core library.
- For Windows-only APIs, ensure code is guarded or documented for portability.

---
_If you are unsure about a workflow or pattern, check the corresponding `*.cs` file in the relevant project folder for examples._
